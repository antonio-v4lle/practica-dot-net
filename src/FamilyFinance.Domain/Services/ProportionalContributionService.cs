using FamilyFinance.Domain.Aggregates.Expenses;
using FamilyFinance.Domain.Aggregates.FamilyBudget;
using FamilyFinance.Domain.ValueObjects;

namespace FamilyFinance.Domain.Services;

/// <summary>
/// Calculates each member's proportional contribution to shared expenses
/// and produces the biweekly budget summary.
/// </summary>
public class ProportionalContributionService
{
    public BudgetSummary CalculateBudgetSummary(
        FamilyGroup familyGroup,
        IReadOnlyList<SharedExpense> sharedExpenses)
    {
        var activeExpenses = sharedExpenses
            .Where(e => e.FamilyGroupId == familyGroup.Id && e.IsActive)
            .ToList();

        var totalMonthly = activeExpenses
            .Aggregate(Money.Zero, (sum, e) => sum.Add(e.MonthlyAmount));

        var memberSummaries = familyGroup.Members.Select(member =>
        {
            var monthlyContribution = member.CalculateContribution(totalMonthly);
            var biweeklyContribution = monthlyContribution.Divide(2);
            var deficit = totalMonthly.Subtract(familyGroup.TotalMonthlyIncome);

            return new MemberBudgetSummary(
                member.Id,
                member.Name,
                member.MonthlyNetIncome,
                member.Proportion,
                monthlyContribution,
                biweeklyContribution,
                deficit.Amount > 0 ? member.CalculateContribution(deficit) : Money.Zero);
        }).ToList();

        var expenseSummaries = activeExpenses.Select(expense =>
        {
            var perMember = familyGroup.Members.Select(m => new MemberExpenseShare(
                m.Id,
                m.Name,
                m.CalculateContribution(expense.MonthlyAmount),
                m.BiweeklyContribution(expense.MonthlyAmount))).ToList();

            return new ExpenseSummary(
                expense.Id,
                expense.Description,
                expense.Category,
                expense.MonthlyAmount,
                expense.FirstFortnightAmount,
                expense.SecondFortnightAmount,
                expense.Schedule?.PaymentDay,
                perMember);
        }).ToList();

        return new BudgetSummary(
            familyGroup.Id,
            familyGroup.Name,
            familyGroup.TotalMonthlyIncome,
            familyGroup.TotalBiweeklyIncome,
            totalMonthly,
            totalMonthly.Divide(2),
            memberSummaries,
            expenseSummaries);
    }
}

public record BudgetSummary(
    Guid FamilyGroupId,
    string FamilyName,
    Money TotalMonthlyIncome,
    Money TotalBiweeklyIncome,
    Money TotalMonthlyExpenses,
    Money TotalBiweeklyExpenses,
    IReadOnlyList<MemberBudgetSummary> Members,
    IReadOnlyList<ExpenseSummary> Expenses)
{
    public Money MonthlyBalance => TotalMonthlyIncome.Subtract(TotalMonthlyExpenses);
    public bool HasDeficit => MonthlyBalance.IsNegative;
}

public record MemberBudgetSummary(
    Guid MemberId,
    string Name,
    Money MonthlyIncome,
    ProportionRatio Proportion,
    Money MonthlyContribution,
    Money BiweeklyContribution,
    Money DeficitContribution);

public record ExpenseSummary(
    Guid ExpenseId,
    string Description,
    ExpenseCategory Category,
    Money MonthlyAmount,
    Money FirstFortnightAmount,
    Money SecondFortnightAmount,
    int? PaymentDay,
    IReadOnlyList<MemberExpenseShare> MemberShares);

public record MemberExpenseShare(
    Guid MemberId,
    string MemberName,
    Money MonthlyShare,
    Money BiweeklyShare);
