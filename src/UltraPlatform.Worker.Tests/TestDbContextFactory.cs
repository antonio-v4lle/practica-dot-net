using Microsoft.EntityFrameworkCore;
using UltraPlatform.Worker.Database;
using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Tests;

/// <summary>
/// Shared factory for InMemory DbContext — each test gets an isolated DB instance.
/// </summary>
public static class TestDbContextFactory
{
    public static AppDbContext Create(string dbName, IEnumerable<DataRecord>? seed = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var ctx = new AppDbContext(options);
        ctx.Database.EnsureCreated();

        if (seed is not null)
        {
            ctx.DataRecords.AddRange(seed);
            ctx.SaveChanges();
        }

        return ctx;
    }

    /// <summary>
    /// Standard seed: mix of active/inactive and today/yesterday records.
    /// </summary>
    public static IEnumerable<DataRecord> StandardSeed(DateOnly today) =>
    [
        new() { Id = "1", Code = "ACTIVE-001", Date = today,            IsActive = true  },
        new() { Id = "2", Code = "INACTIVE-A", Date = today,            IsActive = false },
        new() { Id = "3", Code = "ACTIVE-002", Date = today,            IsActive = true  },
        new() { Id = "4", Code = "INACTIVE-B", Date = today,            IsActive = false },
        new() { Id = "5", Code = "ACTIVE-003", Date = today.AddDays(-1), IsActive = true  },
    ];
}
