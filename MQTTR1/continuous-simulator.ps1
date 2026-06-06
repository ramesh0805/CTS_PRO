# Continuous Live Telemetry Data Simulator
# This script continuously sends telemetry data every 2 minutes
# Press Ctrl+C to stop

$apiUrl = "http://localhost:5297/api/telemetry"
$intervalMinutes = 2

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Continuous Telemetry Simulator" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Sending telemetry data to: $apiUrl" -ForegroundColor Yellow
Write-Host "Interval: Every $intervalMinutes minutes" -ForegroundColor Yellow
Write-Host "Press Ctrl+C to stop" -ForegroundColor Red
Write-Host ""

$devices = @(
	@{ DeviceId = "device-001"; Metric = "temperature"; BaseValue = 22.0; Variance = 5.0; Unit = "°C" },
	@{ DeviceId = "device-002"; Metric = "humidity"; BaseValue = 60.0; Variance = 10.0; Unit = "%" },
	@{ DeviceId = "device-003"; Metric = "pressure"; BaseValue = 1013.0; Variance = 5.0; Unit = "hPa" }
)

$count = 0

try {
	while ($true) {
		foreach ($device in $devices) {
			# Generate random value with variance
			$randomFactor = (Get-Random -Minimum -1.0 -Maximum 1.0)
			$value = $device.BaseValue + ($randomFactor * $device.Variance)
			$value = [Math]::Round($value, 2)

			# Create telemetry payload
			$telemetry = @{
				deviceId = $device.DeviceId
				metric = $device.Metric
				value = $value
				timestamp = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
			} | ConvertTo-Json

			try {
				# Send POST request
				$response = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $telemetry -ContentType "application/json" -ErrorAction Stop
				$count++
				$timestamp = Get-Date -Format "HH:mm:ss"
				Write-Host "[$timestamp] $($device.DeviceId) - $($device.Metric): $value$($device.Unit) ✓" -ForegroundColor Green
			}
			catch {
				Write-Host "[ERROR] Failed to send data: $($_.Exception.Message)" -ForegroundColor Red
			}
		}

		# Wait 2 minutes before next batch
		Write-Host ""
		Write-Host "Next reading in $intervalMinutes minutes..." -ForegroundColor Gray
		Start-Sleep -Seconds ($intervalMinutes * 60)
	}
}
catch {
	Write-Host ""
	Write-Host "=====================================" -ForegroundColor Cyan
	Write-Host "Simulator stopped. Total readings sent: $count" -ForegroundColor Yellow
	Write-Host "=====================================" -ForegroundColor Cyan
}
