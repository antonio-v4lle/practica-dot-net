using FamilyFinance.Application.Credits.Commands;
using FamilyFinance.Application.Credits.Queries;
using FamilyFinance.Application.FamilyGroups.Commands;
using FamilyFinance.Application.Tests.Infrastructure;
using FamilyFinance.Domain.Services;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyFinance.Application.Tests.Features;

/// <summary>
/// FT_Credits_OptimizationStrategies — endpoint de optimización con datos reales del Excel.
/// Valida que Avalancha y Bola de Nieve retornen planes coherentes con recomendación.
/// </summary>
public class FT_Credits_OptimizationStrategies : IClassFixture<InMemoryTestFixture>
{
    private readonly InMemoryTestFixture _fixture;
    public FT_Credits_OptimizationStrategies(InMemoryTestFixture fixture) => _fixture = fixture;

    private async Task<Guid> SetupExcelScenario(IMediator mediator)
    {
        var familyId = await mediator.Send(new CreateFamilyGroupCommand("Familia Principal"));
        var antonioId = await mediator.Send(new AddMemberCommand(familyId, "Antonio", 42_000m));

        await mediator.Send(new AddCreditLineCommand(familyId, antonioId, "TDC BBVA",
            137_400m, 105_357.96m, 36m, 9, 9_000m));
        await mediator.Send(new AddCreditLineCommand(familyId, antonioId, "TDC UN",
            5_000m, 3_881.31m, 42m, 20, 4_500m));

        return familyId;
    }

    [Fact]
    public async Task GetOptimization_WithExcelData_ReturnsBothStrategies()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await SetupExcelScenario(mediator);

        var result = await mediator.Send(
            new GetCreditOptimizationQuery(familyId, 20_000m));

        result.Avalanche.Should().NotBeNull();
        result.Snowball.Should().NotBeNull();
        result.Recommendation.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetOptimization_Avalanche_PrioritizesHighestRateCredit()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await SetupExcelScenario(mediator);

        var result = await mediator.Send(new GetCreditOptimizationQuery(familyId, 20_000m));

        // TDC UN (42%) has higher rate → gets extra payment in Avalanche
        result.Avalanche.Allocations.First().CreditName.Should().Be("TDC UN");
        result.Avalanche.Allocations.First().ExtraPayment.Amount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetOptimization_Snowball_PrioritizesLowestBalanceCredit()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await SetupExcelScenario(mediator);

        var result = await mediator.Send(new GetCreditOptimizationQuery(familyId, 20_000m));

        // TDC UN (3,881) has lower balance → gets extra payment in Snowball
        result.Snowball.Allocations.First().CreditName.Should().Be("TDC UN");
    }

    [Fact]
    public async Task GetOptimization_InsufficientBudget_ReturnsInsufficientFundsFlag()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await SetupExcelScenario(mediator);
        // Min payments: 9000 + 4500 = 13,500 → budget 5,000 is insufficient

        var result = await mediator.Send(new GetCreditOptimizationQuery(familyId, 5_000m));

        result.Avalanche.IsInsufficientFunds.Should().BeTrue();
        result.Recommendation.Should().Contain("insuficiente");
    }

    [Fact]
    public async Task GetOptimization_ExactMinimumBudget_HasNoExtraPayments()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await SetupExcelScenario(mediator);
        // Exact minimum: 9000 + 4500 = 13,500

        var result = await mediator.Send(new GetCreditOptimizationQuery(familyId, 13_500m));

        result.Avalanche.Allocations.Should().AllSatisfy(a =>
            a.ExtraPayment.Amount.Should().Be(0));
    }

    [Fact]
    public async Task GetOptimization_AvalancheAlwaysSavesMoreThanSnowball_WhenRatesDiffer()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await SetupExcelScenario(mediator);

        var result = await mediator.Send(new GetCreditOptimizationQuery(familyId, 20_000m));

        // Avalanche total interest should be ≤ Snowball total interest
        result.Avalanche.TotalInterestCost.Amount.Should()
            .BeLessThanOrEqualTo(result.Snowball.TotalInterestCost.Amount);
    }

    [Fact]
    public async Task GetOptimization_TotalAllocated_EqualsBudget()
    {
        using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var familyId = await SetupExcelScenario(mediator);

        var result = await mediator.Send(new GetCreditOptimizationQuery(familyId, 20_000m));

        result.Avalanche.Allocations.Sum(a => a.TotalMonthlyPayment.Amount)
            .Should().BeApproximately(20_000m, precision: 1m);
    }
}
