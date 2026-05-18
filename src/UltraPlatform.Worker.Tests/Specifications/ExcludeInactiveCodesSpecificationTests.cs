using NSubstitute;
using UltraPlatform.Worker.Models;
using UltraPlatform.Worker.Repositories;
using UltraPlatform.Worker.Specifications.Data;
using UltraPlatform.Worker.Tests;

namespace UltraPlatform.Worker.Tests.Specifications;

public class ExcludeInactiveCodesSpecificationTests
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    // ── Apply ─────────────────────────────────────────────────────────────

    [Fact]
    public void Apply_ExcludesRecordsWithInactiveCodes()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var seed   = TestDbContextFactory.StandardSeed(_today);
        using var ctx = TestDbContextFactory.Create(dbName, seed);

        var repository = Substitute.For<IDataRecordRepository>();
        repository.GetInactiveCodesQueryable()
            .Returns(ctx.DataRecords.Where(r => !r.IsActive).Select(r => r.Code));

        var sut   = new ExcludeInactiveCodesSpecification(repository);
        var query = ctx.DataRecords.AsQueryable();

        // Act
        var result = sut.Apply(query).ToList();

        // Assert
        Assert.All(result, r => Assert.True(r.IsActive));
    }

    [Fact]
    public void Apply_ReturnsAllRecords_WhenNoInactiveCodes()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var seed = new List<DataRecord>
        {
            new() { Id = "1", Code = "A", Date = _today, IsActive = true },
            new() { Id = "2", Code = "B", Date = _today, IsActive = true },
        };
        using var ctx = TestDbContextFactory.Create(dbName, seed);

        var repository = Substitute.For<IDataRecordRepository>();
        repository.GetInactiveCodesQueryable()
            .Returns(Enumerable.Empty<string>().AsQueryable());

        var sut   = new ExcludeInactiveCodesSpecification(repository);
        var query = ctx.DataRecords.AsQueryable();

        // Act
        var result = sut.Apply(query).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Apply_ReturnsEmpty_WhenAllCodesAreInactive()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var seed = new List<DataRecord>
        {
            new() { Id = "1", Code = "INACTIVE-A", Date = _today, IsActive = false },
            new() { Id = "2", Code = "INACTIVE-B", Date = _today, IsActive = false },
        };
        using var ctx = TestDbContextFactory.Create(dbName, seed);

        var repository = Substitute.For<IDataRecordRepository>();
        repository.GetInactiveCodesQueryable()
            .Returns(ctx.DataRecords.Where(r => !r.IsActive).Select(r => r.Code));

        var sut   = new ExcludeInactiveCodesSpecification(repository);
        var query = ctx.DataRecords.AsQueryable();

        // Act
        var result = sut.Apply(query).ToList();

        // Assert
        Assert.Empty(result);
    }
}
