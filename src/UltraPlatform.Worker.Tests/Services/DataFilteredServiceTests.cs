using Microsoft.EntityFrameworkCore;
using NSubstitute;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Models;
using UltraPlatform.Worker.Services;
using UltraPlatform.Worker.Specifications;
using UltraPlatform.Worker.Tests;

namespace UltraPlatform.Worker.Tests.Services;

public class DataFilteredServiceTests
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    private (DataFilteredService sut, IDataService inner, IDataValidationHelper helper) BuildSut(
        string dbName,
        IEnumerable<DataRecord>? seed = null,
        IEnumerable<ISpecification<DataRecord>>? specs = null)
    {
        var ctx    = TestDbContextFactory.Create(dbName, seed);
        var inner  = Substitute.For<IDataService>();
        var helper = Substitute.For<IDataValidationHelper>();

        var sut = new DataFilteredService(
            inner,
            helper,
            specs ?? Enumerable.Empty<ISpecification<DataRecord>>(),
            ctx);

        return (sut, inner, helper);
    }

    // ── ComposeValidation ─────────────────────────────────────────────────

    [Fact]
    public async Task ComposeValidation_ReturnsTrue_WhenBothValidationsPass()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var seed   = new List<DataRecord>
        {
            new() { Id = "1", Code = "ACTIVE-001", Date = _today, IsActive = true }
        };
        var (sut, _, helper) = BuildSut(dbName, seed);
        helper.SecondValidation().Returns(true);

        // Act
        var result = await sut.ComposeValidation();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ComposeValidation_ReturnsFalse_WhenFirstValidationFails()
    {
        // Arrange — no records, FirstValidation returns false (count == 0)
        var dbName = Guid.NewGuid().ToString();
        var (sut, _, helper) = BuildSut(dbName, seed: null);
        helper.SecondValidation().Returns(true);

        // Act
        var result = await sut.ComposeValidation();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ComposeValidation_ReturnsFalse_WhenSecondValidationFails()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var seed   = new List<DataRecord>
        {
            new() { Id = "1", Code = "ACTIVE-001", Date = _today, IsActive = true }
        };
        var (sut, _, helper) = BuildSut(dbName, seed);
        helper.SecondValidation().Returns(false);

        // Act
        var result = await sut.ComposeValidation();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ComposeValidation_DoesNotCallSecondValidation_WhenFirstFails()
    {
        // Arrange — short-circuit: first fails, second should NOT be called
        var dbName = Guid.NewGuid().ToString();
        var (sut, _, helper) = BuildSut(dbName, seed: null);
        helper.SecondValidation().Returns(true);

        // Act
        await sut.ComposeValidation();

        // Assert — SecondValidation never called due to && short-circuit
        await helper.DidNotReceive().SecondValidation();
    }

    [Fact]
    public async Task ComposeValidation_AppliesSpecs_BeforeCountingRecords()
    {
        // Arrange — spec that excludes all records
        var dbName = Guid.NewGuid().ToString();
        var seed   = new List<DataRecord>
        {
            new() { Id = "1", Code = "ACTIVE-001", Date = _today, IsActive = true }
        };

        var excludeAllSpec = Substitute.For<ISpecification<DataRecord>>();
        excludeAllSpec.Apply(Arg.Any<IQueryable<DataRecord>>())
            .Returns(call => ((IQueryable<DataRecord>)call[0]).Where(r => false)); // exclude all

        var (sut, _, helper) = BuildSut(dbName, seed, specs: [excludeAllSpec]);
        helper.SecondValidation().Returns(true);

        // Act
        var result = await sut.ComposeValidation();

        // Assert — spec excluded everything, count == 0 → false
        Assert.False(result);
    }

    // ── Delegate to _inner ────────────────────────────────────────────────

    [Fact]
    public void GetAll_DelegatesToInner()
    {
        // Arrange
        var dbName   = Guid.NewGuid().ToString();
        var expected = new List<DataRecord> { new() { Id = "1", Code = "A", Date = _today, IsActive = true } };
        var (sut, inner, _) = BuildSut(dbName);
        inner.GetAll().Returns(expected);

        // Act
        var result = sut.GetAll();

        // Assert
        Assert.Equal(expected, result);
        inner.Received(1).GetAll();
    }

    [Fact]
    public void GetById_DelegatesToInner()
    {
        // Arrange
        var dbName   = Guid.NewGuid().ToString();
        var expected = new DataRecord { Id = "1", Code = "A", Date = _today, IsActive = true };
        var (sut, inner, _) = BuildSut(dbName);
        inner.GetById("1").Returns(expected);

        // Act
        var result = sut.GetById("1");

        // Assert
        Assert.Equal(expected, result);
        inner.Received(1).GetById("1");
    }
}
