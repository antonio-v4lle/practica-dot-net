using FamilyFinance.Domain.Aggregates.Credits;
using FamilyFinance.Domain.ValueObjects;

namespace FamilyFinance.Domain.Services;

/// <summary>
/// Domain service for credit payment optimization strategies.
/// Implements Avalanche (highest interest first) and Snowball (lowest balance first) methods.
/// </summary>
public class CreditOptimizationService
{
    /// <summary>
    /// Avalanche method: directs extra payments to the highest interest rate debt first.
    /// Minimizes total interest paid and is mathematically optimal.
    /// </summary>
    public OptimizationPlan CalculateAvalanchePlan(
        IReadOnlyList<CreditLine> credits,
        Money totalMonthlyBudget)
    {
        return BuildPlan(
            credits,
            totalMonthlyBudget,
            OptimizationStrategy.Avalanche,
            c => c.AnnualInterestRate.AnnualPercentage,
            descending: true);
    }

    /// <summary>
    /// Snowball method: directs extra payments to the lowest balance debt first.
    /// Provides quicker psychological wins with fewer accounts paid off.
    /// </summary>
    public OptimizationPlan CalculateSnowballPlan(
        IReadOnlyList<CreditLine> credits,
        Money totalMonthlyBudget)
    {
        return BuildPlan(
            credits,
            totalMonthlyBudget,
            OptimizationStrategy.Snowball,
            c => c.CurrentBalance.Amount,
            descending: false);
    }

    private OptimizationPlan BuildPlan(
        IReadOnlyList<CreditLine> credits,
        Money totalMonthlyBudget,
        OptimizationStrategy strategy,
        Func<CreditLine, decimal> prioritySelector,
        bool descending)
    {
        var activeCredits = credits.Where(c => c.CurrentBalance.Amount > 0).ToList();
        if (!activeCredits.Any())
            return OptimizationPlan.Empty(strategy);

        var minimumPaymentsTotal = activeCredits.Sum(c => c.MinimumPayment.Amount);
        if (totalMonthlyBudget.Amount < minimumPaymentsTotal)
            return OptimizationPlan.InsufficientFunds(strategy, Money.Of(minimumPaymentsTotal));

        var extraBudget = totalMonthlyBudget.Amount - minimumPaymentsTotal;

        var ordered = descending
            ? activeCredits.OrderByDescending(prioritySelector).ToList()
            : activeCredits.OrderBy(prioritySelector).ToList();

        var allocations = new List<PaymentAllocation>();
        var remainingExtra = extraBudget;

        foreach (var credit in ordered)
        {
            var extraForThis = remainingExtra > 0 ? remainingExtra : 0;
            var totalPayment = credit.MinimumPayment.Amount + extraForThis;
            remainingExtra = 0; // All extra goes to priority credit

            var monthsToPayoff = credit.EstimateMonthsToPayOff(Money.Of(totalPayment));
            var totalInterest = totalPayment > 0
                ? credit.EstimateTotalInterest(Money.Of(totalPayment))
                : Money.Of(decimal.MaxValue / 2);

            allocations.Add(new PaymentAllocation(
                credit.Id,
                credit.Name,
                credit.CurrentBalance,
                credit.AnnualInterestRate,
                credit.MinimumPayment,
                Money.Of(extraForThis),
                Money.Of(totalPayment),
                monthsToPayoff,
                totalInterest));
        }

        // Baseline: compare against paying minimums only
        var baselineAllocations = activeCredits.Select(c =>
        {
            var months = c.EstimateMonthsToPayOff(c.MinimumPayment);
            var interest = c.EstimateTotalInterest(c.MinimumPayment);
            return new PaymentAllocation(c.Id, c.Name, c.CurrentBalance, c.AnnualInterestRate,
                c.MinimumPayment, Money.Zero, c.MinimumPayment, months, interest);
        }).ToList();

        var baselineTotalInterest = baselineAllocations
            .Where(a => a.EstimatedTotalInterest.Amount < decimal.MaxValue / 2)
            .Sum(a => a.EstimatedTotalInterest.Amount);

        var optimizedTotalInterest = allocations
            .Where(a => a.EstimatedTotalInterest.Amount < decimal.MaxValue / 2)
            .Sum(a => a.EstimatedTotalInterest.Amount);

        var interestSavings = Math.Max(0, baselineTotalInterest - optimizedTotalInterest);

        return new OptimizationPlan(
            strategy,
            allocations,
            Money.Of(optimizedTotalInterest),
            Money.Of(interestSavings),
            allocations.Max(a => a.EstimatedMonthsToPayoff ?? 0));
    }
}

public record PaymentAllocation(
    Guid CreditLineId,
    string CreditName,
    Money CurrentBalance,
    InterestRate AnnualInterestRate,
    Money MinimumPayment,
    Money ExtraPayment,
    Money TotalMonthlyPayment,
    int? EstimatedMonthsToPayoff,
    Money EstimatedTotalInterest);

public record OptimizationPlan(
    OptimizationStrategy Strategy,
    IReadOnlyList<PaymentAllocation> Allocations,
    Money TotalInterestCost,
    Money InterestSavingsVsMinimum,
    int MonthsToFullPayoff)
{
    public static OptimizationPlan Empty(OptimizationStrategy strategy) =>
        new(strategy, [], Money.Zero, Money.Zero, 0);

    public static OptimizationPlan InsufficientFunds(OptimizationStrategy strategy, Money minimumRequired) =>
        new(strategy, [], minimumRequired, Money.Zero, 0) { IsInsufficientFunds = true };

    public bool IsInsufficientFunds { get; init; }
}

public enum OptimizationStrategy
{
    Avalanche,
    Snowball
}
