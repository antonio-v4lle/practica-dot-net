using Microsoft.EntityFrameworkCore;
using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Database;

public class AppDbContext : DbContext
{
    public DbSet<DataRecord> DataRecords => Set<DataRecord>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataRecord>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Code).IsRequired();
        });

        // Seed data for testing
        modelBuilder.Entity<DataRecord>().HasData(
            new DataRecord { Id = "1", Code = "ACTIVE-001", Date = DateOnly.FromDateTime(DateTime.Now), IsActive = true },
            new DataRecord { Id = "2", Code = "TEST",       Date = DateOnly.FromDateTime(DateTime.Now), IsActive = false },
            new DataRecord { Id = "3", Code = "ACTIVE-002", Date = DateOnly.FromDateTime(DateTime.Now), IsActive = true },
            new DataRecord { Id = "4", Code = "TEMP",       Date = DateOnly.FromDateTime(DateTime.Now), IsActive = false },
            new DataRecord { Id = "5", Code = "ACTIVE-003", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), IsActive = true }
        );
    }
}
