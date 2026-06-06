namespace MQTTR1.Models;

public class Telemetry
{
    public int Id { get; set; }

    public int DeviceId { get; set; }

    public required string Metric { get; set; }

    public decimal Value { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Device Device { get; set; } = null!;
}
