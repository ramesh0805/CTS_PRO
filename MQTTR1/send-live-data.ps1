# Send Live Telemetry Data to API
# This script simulates IoT devices sending historical telemetry data
# Data is timestamped with 2-minute intervals going back in time

$apiUrl = "http://localhost:5297/api/telemetry"
$intervalMinutes = 2

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Live Telemetry Data Simulator" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Sending telemetry data to: $apiUrl" -ForegroundColor Yellow
Write-Host "Interval: $intervalMinutes minutes between readings" -ForegroundColor Yellow
Write-Host ""

$devices = @(
	@{ DeviceId = "device-001"; Metric = "temperature"; BaseValue = 22.0; Variance = 5.0 },
	@{ DeviceId = "device-002"; Metric = "humidity"; BaseValue = 60.0; Variance = 10.0 },
	@{ DeviceId = "device-003"; Metric = "pressure"; BaseValue = 1013.0; Variance = 5.0 }
)

$count = 0
$maxReadings = 100  # Send 100 readings per device (covers ~3.3 hours of historical data)

Write-Host "Generating $maxReadings historical readings for each device (2-minute intervals)..." -ForegroundColor Green
Write-Host ""

$now = Get-Date

foreach ($device in $devices) {
	Write-Host "Device: $($device.DeviceId) | Metric: $($device.Metric)" -ForegroundColor Magenta

	for ($i = $maxReadings - 1; $i -ge 0; $i--) {
		# Generate random value with variance
		$randomFactor = (Get-Random -Minimum -1.0 -Maximum 1.0)
		$value = $device.BaseValue + ($randomFactor * $device.Variance)
		$value = [Math]::Round($value, 2)

		# Calculate timestamp going back in time with 2-minute intervals
		$timestamp = $now.AddMinutes(-$i * $intervalMinutes).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")

		# Create telemetry payload
		$telemetry = @{
			deviceId = $device.DeviceId
			metric = $device.Metric
			value = $value
			timestamp = $timestamp
		} | ConvertTo-Json

		try {
			# Send POST request
			$response = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $telemetry -ContentType "application/json" -ErrorAction Stop
			$count++
			Write-Host "  [$count] Sent: $($device.Metric)=$value | Status: OK" -ForegroundColor Green
		}
		catch {
			Write-Host "  [ERROR] Failed to send data: $($_.Exception.Message)" -ForegroundColor Red
		}

		# Small delay between requests (50ms)
		Start-Sleep -Milliseconds 50
	}

	Write-Host ""
}

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Total readings sent: $count" -ForegroundColor Yellow
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Check your frontend at http://localhost:4200 to see the data!" -ForegroundColor Green
