using Microsoft.EntityFrameworkCore;
using MQTTR1.Models;

namespace MQTTR1.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Device> Devices { get; set; }
    public DbSet<Telemetry> Telemetries { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Device configuration
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.DeviceId)
                .IsUnique();

            entity.Property(e => e.DeviceId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });

        // Telemetry configuration
        modelBuilder.Entity<Telemetry>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Composite index for common query patterns (device + metric + time range)
            entity.HasIndex(e => new { e.DeviceId, e.Metric, e.Timestamp })
                .HasDatabaseName("IX_Telemetry_DeviceId_Metric_Timestamp");

            // Index for timestamp-based queries (cleanup, time range queries)
            entity.HasIndex(e => e.Timestamp)
                .HasDatabaseName("IX_Telemetry_Timestamp");

            // Index for device-based queries
            entity.HasIndex(e => e.DeviceId)
                .HasDatabaseName("IX_Telemetry_DeviceId");

            entity.Property(e => e.Metric)
                .IsRequired()
                .HasMaxLength(50); // Reduced from 100 to 50 (temperature/humidity/pressure)

            entity.Property(e => e.Value)
                .IsRequired()
                .HasPrecision(18, 2); // Add precision for decimal storage

            entity.Property(e => e.Timestamp)
                .IsRequired()
                .HasColumnType("datetime2"); // Use datetime2 for better precision

            entity.HasOne(e => e.Device)
                .WithMany(d => d.Telemetries)
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Username)
                .IsUnique();

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                .IsRequired();

            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(20);
        });
    }
}
