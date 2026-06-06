namespace MQTTR1.DTOs;

public class DeviceDto
{
    public required string DeviceId { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
