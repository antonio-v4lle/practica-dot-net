using FamilyFinance.Domain.Aggregates.Credits;
using FamilyFinance.Domain.Services;
using FamilyFinance.Domain.ValueObjects;
using FluentAssertions;

namespace FamilyFinance.Domain.Tests.Services;

/// <summary>
/// UT_CreditOptimizationService_StrategyComparison
/// Valida que Avalancha y Bola de Nieve prioricen créditos correctamente
/// y calculen ahorros de intereses vs pago mínimo.
/// </summary>
public class UT_CreditOptimizationService_StrategyComparison
{
    private readonly CreditOptimizationService _service = new();
    private static readonly Guid FamilyId = Guid.NewGuid();
    private static readonly Guid MemberId = Guid.NewGuid();

    private static List<CreditLine> CreateTwoCredits()
    {
        // Credit A: high rate, high balance
        var tdcBbva = CreditLine.Create(FamilyId, MemberId, "TDC BBVA",
            Money.Of(137_400m), Money.Of(105_357.96m),
            InterestRate.From(36m), 9, Money.Of(9_000m));

        // Credit B: higher rate, low balance
        var tdcUn = CreditLine.Create(FamilyId, MemberId, "TDC UN",
            Money.Of(5_000m), Money.Of(3_881.31m),
            InterestRate.From(42m), 20, Money.Of(4_500m));

        return [tdcBbva, tdcUn];
    }

    [Fact]
    public void Avalanche_PrioritizesHighestInterestRateFirst()
    {
        var credits = CreateTwoCredits();
        var budget = Money.Of(20_000m);

        var plan = _service.CalculateAvalanchePlan(credits, budget);

        // TDC UN (42%) should be first (highest rate)
        plan.Allocations.First().CreditName.Should().Be("TDC UN");
        plan.Allocations.First().ExtraPayment.Amount.Should().BeGreaterThan(0m);
        plan.Allocations.Last().ExtraPayment.Amount.Should().Be(0m);
    }

    [Fact]
    public void Snowball_PrioritizesLowestBalanceFirst()
    {
        var credits = CreateTwoCredits();
        var budget = Money.Of(20_000m);

        var plan = _service.CalculateSnowballPlan(credits, budget);

        // TDC UN (3,881) should be first (lowest balance)
        plan.Allocations.First().CreditName.Should().Be("TDC UN");
    }

    [Fact]
    public void Avalanche_TotalPaymentsEqualsBudget()
    {
        var credits = CreateTwoCredits();
        var budget = Money.Of(20_000m);

        var plan = _service.CalculateAvalanchePlan(credits, budget);

        var totalAllocated = plan.Allocations.Sum(a => a.TotalMonthlyPayment.Amount);
        totalAllocated.Should().BeApproximately(budget.Amount, precision: 1m);
    }

    [Fact]
    public void Avalanche_AllMinimumPaymentsCovered()
    {
        var credits = CreateTwoCredits();
        var budget = Money.Of(20_000m);

        var plan = _service.CalculateAvalanchePlan(credits, budget);

        foreach (var allocation in plan.Allocations)
        {
            allocation.TotalMonthlyPayment.Amount.Should()
                .BeGreaterThanOrEqualTo(allocation.MinimumPayment.Amount);
        }
    }

    [Fact]
    public void InsufficientFunds_BudgetBelowMinimums_ReturnsInsufficientFundsFlag()
    {
        var credits = CreateTwoCredits(); // min payments: 9000 + 4500 = 13500
        var budget = Money.Of(5_000m);   // Below minimums

        var plan = _service.CalculateAvalanchePlan(credits, budget);

        plan.IsInsufficientFunds.Should().BeTrue();
    }

    [Fact]
    public void Avalanche_WithExtraBudget_ReducesMonthsToPayoffVsMinimumOnly()
    {
        var credits = CreateTwoCredits(); // min: 13,500
        var budget = Money.Of(20_000m);  // extra: 6,500

        var planExtra = _service.CalculateAvalanchePlan(credits, budget);
        var planMinOnly = _service.CalculateAvalanchePlan(credits, Money.Of(13_500m));

        // Extra budget reduces payoff time
        planExtra.MonthsToFullPayoff.Should().BeLessThanOrEqualTo(planMinOnly.MonthsToFullPayoff);
        planExtra.MonthsToFullPayoff.Should().BeGreaterThan(0);
    }

    [Fact]
    public void EmptyCredits_ReturnsEmptyPlan()
    {
        var plan = _service.CalculateAvalanchePlan([], Money.Of(10_000m));

        plan.Allocations.Should().BeEmpty();
        plan.MonthsToFullPayoff.Should().Be(0);
    }

    [Fact]
    public void Avalanche_SingleCredit_AllExtraGoesToIt()
    {
        var credits = new List<CreditLine> {
            CreditLine.Create(FamilyId, MemberId, "Única TDC",
                Money.Of(50_000m), Money.Of(30_000m), InterestRate.From(36m), 10, Money.Of(1_500m))
        };
        var budget = Money.Of(3_000m);

        var plan = _service.CalculateAvalanchePlan(credits, budget);

        plan.Allocations.Single().ExtraPayment.Amount.Should().Be(1_500m);
        plan.Allocations.Single().TotalMonthlyPayment.Amount.Should().Be(3_000m);
    }
}
