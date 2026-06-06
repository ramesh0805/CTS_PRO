using Microsoft.EntityFrameworkCore;
using MQTTR1.Data;
using MQTTR1.Models;

namespace MQTTR1.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly ApplicationDbContext _context;

    public DeviceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Device?> GetByIdAsync(int id)
    {
        return await _context.Devices.FindAsync(id);
    }

    public async Task<Device?> GetByDeviceIdAsync(string deviceId)
    {
        return await _context.Devices
            .FirstOrDefaultAsync(d => d.DeviceId == deviceId);
    }

    public async Task<IEnumerable<Device>> GetAllAsync()
    {
        return await _context.Devices
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<string, DateTime?>> GetLastSeenMapAsync(IEnumerable<string> deviceIds)
    {
        var ids = deviceIds.ToList();

        var lastSeenList = await _context.Telemetries
            .AsNoTracking()
            .Where(t => ids.Contains(t.Device.DeviceId))
            .GroupBy(t => t.Device.DeviceId)
            .Select(g => new
            {
                DeviceId = g.Key,
                LastSeen = g.Max(t => (DateTime?)t.Timestamp)
            })
            .ToListAsync();

        var map = ids.ToDictionary(id => id, id => (DateTime?)null);
        foreach (var entry in lastSeenList)
        {
            map[entry.DeviceId] = entry.LastSeen;
        }

        return map;
    }

    public async Task<Device> CreateAsync(Device device)
    {
        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
        return device;
    }

    public async Task<Device> UpdateAsync(Device device)
    {
        device.UpdatedAt = DateTime.UtcNow;
        _context.Devices.Update(device);
        await _context.SaveChangesAsync();
        return device;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null)
            return false;

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string deviceId)
    {
        return await _context.Devices
            .AnyAsync(d => d.DeviceId == deviceId);
    }
}
