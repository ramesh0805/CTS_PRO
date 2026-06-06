@echo off
echo ========================================
echo MQTTR Full Stack Application Launcher
echo ========================================
echo.
echo This will start:
echo 1. Backend .NET API (localhost:5000)
echo 2. Frontend Angular App (localhost:4200)
echo.
echo Press any key to continue or Ctrl+C to cancel...
pause >nul

echo.
echo ========================================
echo Starting Backend API...
echo ========================================
start "MQTTR Backend" powershell -Command "cd '%~dp0'; dotnet run --project MQTTR1\MQTTR1.csproj"

echo.
echo Waiting 5 seconds for backend to start...
timeout /t 5 /nobreak >nul

echo.
echo ========================================
echo Starting Frontend...
echo ========================================
start "MQTTR Frontend" powershell -Command "cd '%~dp0mqttr-frontend'; npm start"

echo.
echo ========================================
echo Both services are starting!
echo ========================================
echo.
echo Backend API will be at: http://localhost:5000
echo Swagger UI will be at: http://localhost:5000/swagger
echo Frontend will be at: http://localhost:4200
echo.
echo Check the separate windows for each service.
echo Press any key to exit this launcher...
pause >nul
