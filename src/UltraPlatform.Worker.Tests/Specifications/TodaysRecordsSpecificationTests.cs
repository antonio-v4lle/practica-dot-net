using NSubstitute;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Models;
using UltraPlatform.Worker.Specifications.Data;
using UltraPlatform.Worker.Tests;

namespace UltraPlatform.Worker.Tests.Specifications;

public class TodaysRecordsSpecificationTests
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    private (TodaysRecordsSpecification spec, IUtilityService utility) BuildSut()
    {
        var utility = Substitute.For<IUtilityService>();
        utility.GetToday().Returns(_today);
        return (new TodaysRecordsSpecification(utility), utility);
    }

    // ── Apply ─────────────────────────────────────────────────────────────

    [Fact]
    public void Apply_ReturnsOnlyTodaysRecords_WhenMixedDatesExist()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var seed   = TestDbContextFactory.StandardSeed(_today);
        using var ctx = TestDbContextFactory.Create(dbName, seed);
        var (sut, _) = BuildSut();
        var query = ctx.DataRecords.AsQueryable();

        // Act
        var result = sut.Apply(query).ToList();

        // Assert
        Assert.All(result, r => Assert.Equal(_today, r.Date));
    }

    [Fact]
    public void Apply_ExcludesYesterdaysRecords()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var seed   = TestDbContextFactory.StandardSeed(_today);
        using var ctx = TestDbContextFactory.Create(dbName, seed);
        var (sut, _) = BuildSut();
        var query = ctx.DataRecords.AsQueryable();

        // Act
        var result = sut.Apply(query).ToList();

        // Assert
        Assert.DoesNotContain(result, r => r.Date != _today);
    }

    [Fact]
    public void Apply_ReturnsEmpty_WhenNoRecordsForToday()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var yesterday = _today.AddDays(-1);
        var seed = new List<DataRecord>
        {
            new() { Id = "1", Code = "OLD", Date = yesterday, IsActive = true }
        };
        using var ctx = TestDbContextFactory.Create(dbName, seed);
        var (sut, _) = BuildSut();
        var query = ctx.DataRecords.AsQueryable();

        // Act
        var result = sut.Apply(query).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Apply_UsesUtilityServiceForDate_NotSystemDateTime()
    {
        // Arrange — utility returns a fixed date, not real today
        var fixedDate = new DateOnly(2020, 1, 1);
        var utility   = Substitute.For<IUtilityService>();
        utility.GetToday().Returns(fixedDate);
        var sut = new TodaysRecordsSpecification(utility);

        var dbName = Guid.NewGuid().ToString();
        var seed = new List<DataRecord>
        {
            new() { Id = "1", Code = "OLD",  Date = fixedDate,              IsActive = true },
            new() { Id = "2", Code = "NEW",  Date = DateOnly.FromDateTime(DateTime.Now), IsActive = true }
        };
        using var ctx = TestDbContextFactory.Create(dbName, seed);
        var query = ctx.DataRecords.AsQueryable();

        // Act
        var result = sut.Apply(query).ToList();

        // Assert — only the fixed date record returned
        Assert.Single(result);
        Assert.Equal("OLD", result[0].Code);
    }
}
