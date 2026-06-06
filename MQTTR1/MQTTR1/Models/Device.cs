namespace MQTTR1.Models;

public class Device
{
    public int Id { get; set; }

    public required string DeviceId { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Telemetry> Telemetries { get; set; } = new List<Telemetry>();
}
