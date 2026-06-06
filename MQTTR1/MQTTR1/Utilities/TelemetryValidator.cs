namespace MQTTR1.Utilities;

public static class TelemetryValidator
{
    public static bool IsValidMetricName(string metric)
    {
        if (string.IsNullOrWhiteSpace(metric))
            return false;

        if (metric.Length > 100)
            return false;

        // Metric name should only contain alphanumeric, underscore, hyphen, and dot
        return metric.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.');
    }

    public static bool IsValidDeviceId(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            return false;

        if (deviceId.Length > 100)
            return false;

        return deviceId.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-');
    }

    public static bool IsValidValue(decimal value)
    {
        return value != 0m || value == 0m; // Decimal is always valid unless there's an issue
    }

    public static bool IsValidTimestamp(DateTime? timestamp)
    {
        if (!timestamp.HasValue)
            return true; // Null is valid, will use current time

        // Timestamp should not be in the future (with 5-minute tolerance)
        if (timestamp.Value > DateTime.UtcNow.AddMinutes(5))
            return false;

        // Timestamp should not be too old (more than 1 year)
        if (timestamp.Value < DateTime.UtcNow.AddYears(-1))
            return false;

        return true;
    }
}
