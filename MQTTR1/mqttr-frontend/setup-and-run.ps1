# MQTTR Frontend - Installation and Startup Script
# Run this script from the mqttr-frontend directory

Write-Host "================================" -ForegroundColor Cyan
Write-Host "MQTTR Frontend Setup & Startup" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Check if Node.js is installed
Write-Host "Checking Node.js installation..." -ForegroundColor Yellow
try {
	$nodeVersion = node --version
	Write-Host "✓ Node.js version: $nodeVersion" -ForegroundColor Green
} catch {
	Write-Host "✗ Node.js is not installed!" -ForegroundColor Red
	Write-Host "Please install Node.js from https://nodejs.org/" -ForegroundColor Red
	exit 1
}

# Check if npm is installed
Write-Host "Checking npm installation..." -ForegroundColor Yellow
try {
	$npmVersion = npm --version
	Write-Host "✓ npm version: $npmVersion" -ForegroundColor Green
} catch {
	Write-Host "✗ npm is not installed!" -ForegroundColor Red
	exit 1
}

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan

# Check if node_modules exists
if (Test-Path "node_modules") {
	Write-Host "node_modules folder already exists." -ForegroundColor Yellow
	$response = Read-Host "Do you want to reinstall dependencies? (y/n)"
	if ($response -eq 'y') {
		Write-Host "Removing old node_modules..." -ForegroundColor Yellow
		Remove-Item -Path "node_modules" -Recurse -Force
		Remove-Item -Path "package-lock.json" -Force -ErrorAction SilentlyContinue
		Write-Host "Installing dependencies..." -ForegroundColor Yellow
		npm install
	}
} else {
	Write-Host "Installing dependencies..." -ForegroundColor Yellow
	npm install
}

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Ask if user wants to start the dev server
$startServer = Read-Host "Do you want to start the development server now? (y/n)"

if ($startServer -eq 'y') {
	Write-Host ""
	Write-Host "================================" -ForegroundColor Cyan
	Write-Host "Starting Development Server..." -ForegroundColor Yellow
	Write-Host "================================" -ForegroundColor Cyan
	Write-Host ""
	Write-Host "IMPORTANT:" -ForegroundColor Red
	Write-Host "1. Make sure your backend API is running on http://localhost:5000" -ForegroundColor Yellow
	Write-Host "2. The frontend will be available at http://localhost:4200" -ForegroundColor Yellow
	Write-Host "3. Press Ctrl+C to stop the server" -ForegroundColor Yellow
	Write-Host ""
	Start-Sleep -Seconds 3
	npm start
} else {
	Write-Host ""
	Write-Host "To start the development server later, run:" -ForegroundColor Yellow
	Write-Host "  npm start" -ForegroundColor Cyan
	Write-Host ""
	Write-Host "To build for production, run:" -ForegroundColor Yellow
	Write-Host "  npm run build" -ForegroundColor Cyan
	Write-Host ""
}
