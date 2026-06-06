# Realistic Telemetry Data Generator
# Generates smooth, realistic sensor readings that mimic real-world patterns
# Data covers 24 hours with 2-minute intervals (720 readings per device)

$apiUrl = "http://localhost:5297/api/telemetry"
$intervalMinutes = 2
$hoursOfData = 24
$maxReadings = ($hoursOfData * 60) / $intervalMinutes  # 720 readings for 24 hours

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Realistic Telemetry Data Generator" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Generating $hoursOfData hours of realistic data" -ForegroundColor Yellow
Write-Host "Interval: $intervalMinutes minutes ($maxReadings readings per device)" -ForegroundColor Yellow
Write-Host ""

# Device configurations with realistic patterns
$devices = @(
	@{ 
		DeviceId = "device-001"
		Metric = "temperature"
		BaseValue = 21.0
		DailyVariation = 3.0  # Temperature varies by 3°C throughout the day
		NoiseLevel = 0.2      # Small random fluctuations
		Pattern = "sine"      # Sine wave for day/night cycle
	},
	@{ 
		DeviceId = "device-002"
		Metric = "humidity"
		BaseValue = 55.0
		DailyVariation = 15.0  # Humidity varies more
		NoiseLevel = 0.5
		Pattern = "inverse-sine"  # Inverse of temperature
	},
	@{ 
		DeviceId = "device-003"
		Metric = "pressure"
		BaseValue = 1013.25
		DailyVariation = 3.0   # Pressure changes gradually
		NoiseLevel = 0.3
		Pattern = "slow-wave"  # Slower wave pattern
	}
)

$count = 0
$now = Get-Date

Write-Host "Generating realistic sensor patterns..." -ForegroundColor Green
Write-Host ""

foreach ($device in $devices) {
	Write-Host "Device: $($device.DeviceId) | Metric: $($device.Metric)" -ForegroundColor Magenta

	$previousValue = $device.BaseValue

	for ($i = $maxReadings - 1; $i -ge 0; $i--) {
		# Calculate time-based pattern (0 to 2π for full day cycle)
		$timePhase = (($maxReadings - $i) / $maxReadings) * 2 * [Math]::PI

		# Generate smooth pattern based on device type
		switch ($device.Pattern) {
			"sine" {
				# Temperature: higher during day, lower at night
				$patternValue = [Math]::Sin($timePhase) * $device.DailyVariation
			}
			"inverse-sine" {
				# Humidity: inverse of temperature (higher at night)
				$patternValue = -[Math]::Sin($timePhase) * $device.DailyVariation
			}
			"slow-wave" {
				# Pressure: slower changes (half frequency)
				$patternValue = [Math]::Sin($timePhase / 2) * $device.DailyVariation
			}
		}

		# Add small random noise for realism
		$noise = (Get-Random -Minimum -1.0 -Maximum 1.0) * $device.NoiseLevel

		# Calculate new value (smooth transition from previous value)
		$targetValue = $device.BaseValue + $patternValue + $noise
		$smoothingFactor = 0.85
		$value = ($previousValue * $smoothingFactor) + ($targetValue * (1 - $smoothingFactor))
		$value = [Math]::Round($value, 2)

		# Update previous value for next iteration
		$previousValue = $value

		# Calculate timestamp
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

			# Show progress every 50 readings
			if ($count % 50 -eq 0) {
				Write-Host "  [$count] Progress: $($device.Metric)=$value" -ForegroundColor Green
			}
		}
		catch {
			Write-Host "  [ERROR] Failed to send data: $($_.Exception.Message)" -ForegroundColor Red
		}

		# Small delay to avoid overwhelming the API
		if ($count % 10 -eq 0) {
			Start-Sleep -Milliseconds 100
		}
	}

	Write-Host "  Completed $($device.Metric): $(($maxReadings)) readings sent" -ForegroundColor Cyan
	Write-Host ""
}

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Total readings sent: $count" -ForegroundColor Yellow
Write-Host "Time span: $hoursOfData hours" -ForegroundColor Yellow
Write-Host "Interval: $intervalMinutes minutes" -ForegroundColor Yellow
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Data covers 24 hours with realistic patterns:" -ForegroundColor Green
Write-Host "  - Temperature: Day/night cycle (higher during day)" -ForegroundColor White
Write-Host "  - Humidity: Inverse cycle (higher at night)" -ForegroundColor White
Write-Host "  - Pressure: Slow atmospheric changes" -ForegroundColor White
Write-Host ""
Write-Host "View at: http://localhost:4200 (X-axis will show hours)" -ForegroundColor Cyan
