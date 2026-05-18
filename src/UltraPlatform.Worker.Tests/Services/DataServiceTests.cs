using NSubstitute;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Models;
using UltraPlatform.Worker.Services;

namespace UltraPlatform.Worker.Tests.Services;

public class DataServiceTests
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    private (DataService sut, IDataValidationHelper helper) BuildSut()
    {
        var helper = Substitute.For<IDataValidationHelper>();
        return (new DataService(helper), helper);
    }

    // ── Add ───────────────────────────────────────────────────────────────

    [Fact]
    public void Add_AddsRecord_WhenIdDoesNotExist()
    {
        // Arrange
        var (sut, _) = BuildSut();
        var record   = new DataRecord { Id = "1", Code = "A", Date = _today, IsActive = true };

        // Act
        sut.Add(record);

        // Assert
        Assert.Single(sut.GetAll());
    }

    [Fact]
    public void Add_ThrowsInvalidOperationException_WhenIdAlreadyExists()
    {
        // Arrange
        var (sut, _) = BuildSut();
        var record   = new DataRecord { Id = "1", Code = "A", Date = _today, IsActive = true };
        sut.Add(record);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sut.Add(record));
    }

    // ── GetById ───────────────────────────────────────────────────────────

    [Fact]
    public void GetById_ReturnsRecord_WhenExists()
    {
        // Arrange
        var (sut, _) = BuildSut();
        var record   = new DataRecord { Id = "1", Code = "A", Date = _today, IsActive = true };
        sut.Add(record);

        // Act
        var result = sut.GetById("1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1", result.Id);
    }

    [Fact]
    public void GetById_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var (sut, _) = BuildSut();

        // Act
        var result = sut.GetById("nonexistent");

        // Assert
        Assert.Null(result);
    }

    // ── Update ────────────────────────────────────────────────────────────

    [Fact]
    public void Update_UpdatesRecord_WhenExists()
    {
        // Arrange
        var (sut, _) = BuildSut();
        var record   = new DataRecord { Id = "1", Code = "ORIGINAL", Date = _today, IsActive = true };
        sut.Add(record);
        var updated = record with { Code = "UPDATED" };

        // Act
        sut.Update(updated);

        // Assert
        Assert.Equal("UPDATED", sut.GetById("1")!.Code);
    }

    [Fact]
    public void Update_ThrowsInvalidOperationException_WhenNotExists()
    {
        // Arrange
        var (sut, _) = BuildSut();
        var record   = new DataRecord { Id = "99", Code = "X", Date = _today, IsActive = true };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sut.Update(record));
    }

    // ── Delete ────────────────────────────────────────────────────────────

    [Fact]
    public void Delete_RemovesRecord_WhenExists()
    {
        // Arrange
        var (sut, _) = BuildSut();
        var record   = new DataRecord { Id = "1", Code = "A", Date = _today, IsActive = true };
        sut.Add(record);

        // Act
        sut.Delete("1");

        // Assert
        Assert.Empty(sut.GetAll());
    }

    [Fact]
    public void Delete_ThrowsInvalidOperationException_WhenNotExists()
    {
        // Arrange
        var (sut, _) = BuildSut();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sut.Delete("nonexistent"));
    }

    // ── GetTodaysRecords ──────────────────────────────────────────────────

    [Fact]
    public void GetTodaysRecords_ReturnsOnlyTodaysRecords()
    {
        // Arrange
        var (sut, _) = BuildSut();
        sut.Add(new DataRecord { Id = "1", Code = "TODAY",     Date = _today,            IsActive = true });
        sut.Add(new DataRecord { Id = "2", Code = "YESTERDAY", Date = _today.AddDays(-1), IsActive = true });

        // Act
        var result = sut.GetTodaysRecords().ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("TODAY", result[0].Code);
    }

    // ── ComposeValidation ─────────────────────────────────────────────────

    [Fact]
    public async Task ComposeValidation_ReturnsTrue_WhenBothValidationsPass()
    {
        // Arrange — even count (0 % 2 == 0) → FirstValidation true
        var (sut, helper) = BuildSut();
        helper.SecondValidation().Returns(true);

        // Act
        var result = await sut.ComposeValidation();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ComposeValidation_ReturnsFalse_WhenSecondValidationFails()
    {
        // Arrange — even count → FirstValidation true, but SecondValidation false
        var (sut, helper) = BuildSut();
        helper.SecondValidation().Returns(false);

        // Act
        var result = await sut.ComposeValidation();

        // Assert
        Assert.False(result);
    }
}
