# ⏰ IST Time Verification Test

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   India Standard Time Verification" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get current UTC time
$utcNow = [DateTime]::UtcNow
Write-Host "Current UTC Time:" -ForegroundColor Yellow
Write-Host "  $($utcNow.ToString('yyyy-MM-dd HH:mm:ss')) UTC" -ForegroundColor White
Write-Host ""

# Convert to IST (UTC +5:30)
$istOffset = [TimeSpan]::FromHours(5.5)
$istTime = $utcNow.Add($istOffset)
Write-Host "Correct IST Time (Kolkata):" -ForegroundColor Green
Write-Host "  $($istTime.ToString('dddd, MMMM dd, yyyy, hh:mm:ss tt')) IST" -ForegroundColor White
Write-Host ""

# What the dashboard should show
Write-Host "Dashboard Should Display:" -ForegroundColor Cyan
Write-Host "  $($istTime.ToString('dddd, MMMM dd, yyyy, hh:mm:ss tt')) IST (Kolkata)" -ForegroundColor Magenta
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Verification Steps:" -ForegroundColor Yellow
Write-Host "1. Open http://localhost:4200" -ForegroundColor White
Write-Host "2. Login: admin / admin123" -ForegroundColor White
Write-Host "3. Check time at top of dashboard" -ForegroundColor White
Write-Host "4. Compare with time shown above" -ForegroundColor White
Write-Host "5. They should match (within 1 second)" -ForegroundColor White
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Online verification
Write-Host "Online Verification:" -ForegroundColor Yellow
Write-Host "  Compare with: https://time.is/IST" -ForegroundColor White
Write-Host "  Or Google: 'India time now'" -ForegroundColor White
Write-Host ""

# Show timezone info
Write-Host "IST Timezone Details:" -ForegroundColor Yellow
Write-Host "  IANA Code:    Asia/Kolkata" -ForegroundColor White
Write-Host "  UTC Offset:   +05:30" -ForegroundColor White
Write-Host "  DST:          Not observed" -ForegroundColor White
Write-Host "  Coverage:     All of India" -ForegroundColor White
Write-Host ""

Write-Host "✅ Verification script complete!" -ForegroundColor Green
Write-Host ""
