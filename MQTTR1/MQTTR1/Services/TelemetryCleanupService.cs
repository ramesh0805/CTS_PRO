using Microsoft.EntityFrameworkCore;
using MQTTR1.Data;
using MQTTR1.Models;

namespace MQTTR1.Services;

public class TelemetryCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelemetryCleanupService> _logger;
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1); // Run every hour

    public TelemetryCleanupService(
        IServiceProvider serviceProvider,
        ILogger<TelemetryCleanupService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Telemetry Cleanup Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOldDataAsync();
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during telemetry cleanup.");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait 5 minutes on error
            }
        }
    }

    private async Task CleanupOldDataAsync()
    {
        var enableCleanup = _configuration.GetValue<bool>("TelemetrySettings:EnableDataCleanup", false);

        if (!enableCleanup)
        {
            _logger.LogInformation("Telemetry data cleanup is disabled. All historical data is being retained for analysis.");
            return;
        }

        var retentionDays = _configuration.GetValue<int>("TelemetrySettings:RetentionDays", 365);
        var cutoffTime = DateTime.UtcNow.AddDays(-retentionDays);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            var oldRecords = await context.Telemetries
                .Where(t => t.Timestamp < cutoffTime)
                .ToListAsync();

            if (oldRecords.Any())
            {
                context.Telemetries.RemoveRange(oldRecords);
                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "Cleaned up {Count} telemetry records older than {RetentionDays} days (cutoff: {CutoffTime})",
                    oldRecords.Count,
                    retentionDays,
                    cutoffTime);
            }
            else
            {
                _logger.LogInformation("No old records to clean up.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old telemetry data.");
        }
    }
}
