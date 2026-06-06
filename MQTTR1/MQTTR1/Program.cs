using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MQTTR1.Data;
using MQTTR1.Repositories;
using MQTTR1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Disable detailed logging for better performance
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(false);
        options.EnableDetailedErrors(false);
    }
});

// Previous InMemory configuration (now disabled)
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseInMemoryDatabase("TelemetryDb"));

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForJWTTokenGeneration12345678";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register repositories
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();

// Register services
builder.Services.AddScoped<TelemetryIngestor>();
builder.Services.AddScoped<AuthService>();

// Register background services
builder.Services.AddHostedService<TelemetryCleanupService>();
builder.Services.AddHostedService<LiveTelemetryGeneratorService>();

// Add CORS for future frontend integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Response Caching for better performance
builder.Services.AddResponseCaching();

// Add Memory Caching for frequently accessed data
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed initial data for testing
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Ensure database is created
    context.Database.EnsureCreated();

    // Seed devices if none exist (NO RANDOM TELEMETRY DATA)
    if (!context.Devices.Any())
    {
        var device1 = new MQTTR1.Models.Device 
        { 
            DeviceId = "device-001", 
            Name = "Smart Thermostat - Living Room", 
            Description = "Nest Learning Thermostat monitoring living room temperature",
            CreatedAt = DateTime.UtcNow
        };

        var device2 = new MQTTR1.Models.Device 
        { 
            DeviceId = "device-002", 
            Name = "Humidity Monitor - Basement", 
            Description = "Honeywell humidity sensor for basement moisture control",
            CreatedAt = DateTime.UtcNow
        };

        var device3 = new MQTTR1.Models.Device 
        { 
            DeviceId = "device-003", 
            Name = "Barometric Pressure Sensor - Rooftop", 
            Description = "Weather station barometric pressure sensor on rooftop",
            CreatedAt = DateTime.UtcNow
        };

        context.Devices.AddRange(device1, device2, device3);
        context.SaveChanges();

        Console.WriteLine($"Seeded {3} devices. Use ConsolePublisher to send live telemetry data.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// Enable Response Caching middleware
app.UseResponseCaching();

// Disable HTTPS redirection for development (using HTTP only)
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
