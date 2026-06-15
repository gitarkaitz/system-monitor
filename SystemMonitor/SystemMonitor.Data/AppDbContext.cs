using Microsoft.EntityFrameworkCore;
using SystemMonitor.Data.Models;

namespace SystemMonitor.Data
{
    public class AppDbContext : DbContext
    {
        #region Attributes

        public DbSet<MetricSnapshot> MetricSnapshots { get; set; }
        public DbSet<ProcessSnapshot> ProcessSnapshots { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<AlertEvent> AlertEvents { get; set; }

        #endregion

        #region Constructors

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        #endregion

        #region Configuration

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MetricSnapshot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CapturedAt).IsRequired();
                entity.Property(e => e.CpuUsagePercent).IsRequired();
                entity.Property(e => e.MemoryUsedBytes).IsRequired();
                entity.Property(e => e.MemoryTotalBytes).IsRequired();
                entity.Property(e => e.NetworkBytesReceived).IsRequired();
                entity.Property(e => e.NetworkBytesSent).IsRequired();
                entity.Property(e => e.DiskBytesRead).IsRequired();
                entity.Property(e => e.DiskBytesWritten).IsRequired();
            });

            modelBuilder.Entity<ProcessSnapshot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProcessId).IsRequired();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
                entity.Property(e => e.CapturedAt).IsRequired();
                entity.Property(e => e.CpuUsagePercent).IsRequired();
                entity.Property(e => e.MemoryBytes).IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });

            modelBuilder.Entity<Alert>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MetricType).IsRequired();
                entity.Property(e => e.Threshold).IsRequired();
                entity.Property(e => e.Condition).IsRequired();
                entity.Property(e => e.IsEnabled).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasMany(e => e.Events)
                      .WithOne(e => e.Alert)
                      .HasForeignKey(e => e.AlertId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AlertEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AlertId).IsRequired();
                entity.Property(e => e.TriggeredValue).IsRequired();
                entity.Property(e => e.TriggeredAt).IsRequired();
                entity.Property(e => e.IsAcknowledged).IsRequired();
            });
        }

        #endregion
    }
}
