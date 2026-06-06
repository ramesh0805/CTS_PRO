# Quick Start Script for MQTT Telemetry Collector

Write-Host "╔════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║      MQTT Telemetry Collector - Quick Start           ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

Write-Host "This script will start both the API and Console Publisher" -ForegroundColor Yellow
Write-Host ""

# Start API in a new window
Write-Host "[1/2] Starting API Server..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\MQTTR1'; Write-Host 'Starting MQTT Telemetry API...' -ForegroundColor Cyan; dotnet run"

# Wait a bit for API to start
Write-Host "Waiting for API to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Start Console Publisher in a new window
Write-Host "[2/2] Starting Console Publisher..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\ConsolePublisher'; Write-Host 'Starting Console Publisher...' -ForegroundColor Cyan; dotnet run"

Write-Host ""
Write-Host "✓ Both applications started!" -ForegroundColor Green
Write-Host ""
Write-Host "API Server: Running in separate window" -ForegroundColor Cyan
Write-Host "Console Publisher: Running in separate window" -ForegroundColor Cyan
Write-Host ""
Write-Host "You can now use the Console Publisher to send telemetry to the API." -ForegroundColor Yellow
Write-Host "Press any key to exit this script (applications will continue running)..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
