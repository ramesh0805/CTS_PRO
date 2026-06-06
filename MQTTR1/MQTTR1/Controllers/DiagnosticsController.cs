using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MQTTR1.Data;
using Microsoft.EntityFrameworkCore;

namespace MQTTR1.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class DiagnosticsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DiagnosticsController> _logger;

    public DiagnosticsController(ApplicationDbContext context, ILogger<DiagnosticsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Check live data ingestion - verify records are being stored
    /// </summary>
    [HttpGet("health")]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            // Get database stats
            var deviceCount = await _context.Devices.CountAsync();
            var telemetryCount = await _context.Telemetries.CountAsync();
            var tenHoursAgo = DateTime.UtcNow.AddHours(-10);
            var recentTelemetryCount = await _context.Telemetries
                .Where(t => t.Timestamp >= tenHoursAgo)
                .CountAsync();

            // Get latest telemetry by device
            var latestByDevice = await _context.Telemetries
                .AsNoTracking()
                .Include(t => t.Device)
                .GroupBy(t => t.Device.DeviceId)
                .Select(g => new
                {
                    DeviceId = g.Key,
                    LatestTimestamp = g.Max(t => t.Timestamp),
                    RecordCount = g.Count(),
                    Metrics = g.Select(t => t.Metric).Distinct().ToList()
                })
                .OrderByDescending(x => x.LatestTimestamp)
                .ToListAsync();

            return Ok(new
            {
                status = "healthy",
                database = new
                {
                    devices = deviceCount,
                    totalTelemetry = telemetryCount,
                    last10Hours = recentTelemetryCount,
                    deviceStats = latestByDevice
                },
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    /// <summary>
    /// Get recent telemetry samples (last 50 records)
    /// </summary>
    [HttpGet("recent-telemetry")]
    public async Task<IActionResult> GetRecentTelemetry()
    {
        try
        {
            var recent = await _context.Telemetries
                .AsNoTracking()
                .Include(t => t.Device)
                .OrderByDescending(t => t.Timestamp)
                .Take(50)
                .Select(t => new
                {
                    id = t.Id,
                    deviceId = t.Device.DeviceId,
                    metric = t.Metric,
                    value = t.Value,
                    timestamp = t.Timestamp
                })
                .ToListAsync();

            return Ok(new
            {
                count = recent.Count,
                data = recent,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch recent telemetry");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    /// <summary>
    /// Get telemetry count by device and metric
    /// </summary>
    [HttpGet("telemetry-distribution")]
    public async Task<IActionResult> GetTelemetryDistribution()
    {
        try
        {
            var distribution = await _context.Telemetries
                .AsNoTracking()
                .Include(t => t.Device)
                .GroupBy(t => new { t.Device.DeviceId, t.Metric })
                .Select(g => new
                {
                    deviceId = g.Key.DeviceId,
                    metric = g.Key.Metric,
                    count = g.Count(),
                    min = g.Min(t => t.Value),
                    max = g.Max(t => t.Value),
                    avg = g.Average(t => t.Value),
                    lastValue = g.OrderByDescending(t => t.Timestamp).First().Value,
                    lastTimestamp = g.Max(t => t.Timestamp)
                })
                .OrderBy(x => x.deviceId)
                .ThenBy(x => x.metric)
                .ToListAsync();

            return Ok(new
            {
                count = distribution.Count,
                data = distribution,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch telemetry distribution");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}
