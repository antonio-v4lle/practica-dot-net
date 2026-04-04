using FamilyFinance.Domain.Aggregates.Expenses;
using FamilyFinance.Domain.Aggregates.FamilyBudget;
using FamilyFinance.Domain.Services;
using FamilyFinance.Domain.ValueObjects;
using FluentAssertions;

namespace FamilyFinance.Domain.Tests.Services;

/// <summary>
/// UT_ProportionalContributionService_BudgetSummary
/// Valida que el resumen quincenal muestre aportaciones proporcionales correctas por miembro.
/// </summary>
public class UT_ProportionalContributionService_BudgetSummary
{
    private readonly ProportionalContributionService _service = new();
    private static readonly Guid FamilyId = Guid.NewGuid();

    private static (FamilyGroup group, List<SharedExpense> expenses) CreateScenario()
    {
        var group = FamilyGroup.Create("Familia Principal");
        group.AddMember("Antonio", Money.Of(42_000m));
        group.AddMember("Adriana", Money.Of(17_600m));

        var expenses = new List<SharedExpense>
        {
            SharedExpense.Create(group.Id, "Hipoteca", ExpenseCategory.Patrimonio,
                Money.Of(7_100m), true, 19),
            SharedExpense.Create(group.Id, "Automóvil", ExpenseCategory.Patrimonio,
                Money.Of(11_800m), true, 4),
            SharedExpense.Create(group.Id, "Agua", ExpenseCategory.Servicios,
                Money.Of(250m), true, 1),
        };

        return (group, expenses);
    }

    [Fact]
    public void CalculateBudgetSummary_TwoMembers_ReturnsBothMemberSummaries()
    {
        var (group, expenses) = CreateScenario();

        var summary = _service.CalculateBudgetSummary(group, expenses);

        summary.Members.Should().HaveCount(2);
        summary.Members.Should().Contain(m => m.Name == "Antonio");
        summary.Members.Should().Contain(m => m.Name == "Adriana");
    }

    [Fact]
    public void CalculateBudgetSummary_TotalExpenses_SumsAllActiveExpenses()
    {
        var (group, expenses) = CreateScenario();

        var summary = _service.CalculateBudgetSummary(group, expenses);

        summary.TotalMonthlyExpenses.Amount.Should().Be(19_150m); // 7100 + 11800 + 250
        summary.TotalBiweeklyExpenses.Amount.Should().Be(9_575m);
    }

    [Fact]
    public void CalculateBudgetSummary_MemberContributions_SumToTotalExpenses()
    {
        var (group, expenses) = CreateScenario();

        var summary = _service.CalculateBudgetSummary(group, expenses);

        var totalContributions = summary.Members.Sum(m => m.MonthlyContribution.Amount);
        totalContributions.Should().BeApproximately(summary.TotalMonthlyExpenses.Amount, precision: 1m);
    }

    [Fact]
    public void CalculateBudgetSummary_BiweeklyContribution_IsHalfOfMonthly()
    {
        var (group, expenses) = CreateScenario();

        var summary = _service.CalculateBudgetSummary(group, expenses);

        foreach (var member in summary.Members)
        {
            member.BiweeklyContribution.Amount.Should()
                .BeApproximately(member.MonthlyContribution.Amount / 2, precision: 1m);
        }
    }

    [Fact]
    public void CalculateBudgetSummary_WithSuspendedExpense_ExcludesItFromTotal()
    {
        var (group, expenses) = CreateScenario();
        var suspendedExpense = SharedExpense.Create(group.Id, "Leonel", ExpenseCategory.Educacion,
            Money.Zero, false);
        suspendedExpense.Suspend();
        expenses.Add(suspendedExpense);

        var summary = _service.CalculateBudgetSummary(group, expenses);

        // Suspended expense (inactive) should not appear in summary
        summary.Expenses.Should().NotContain(e => e.Description == "Leonel");
        summary.TotalMonthlyExpenses.Amount.Should().Be(19_150m);
    }

    [Fact]
    public void CalculateBudgetSummary_ExpenseSummary_ContainsPerMemberShares()
    {
        var (group, expenses) = CreateScenario();

        var summary = _service.CalculateBudgetSummary(group, expenses);

        var hipoteca = summary.Expenses.First(e => e.Description == "Hipoteca");
        hipoteca.MemberShares.Should().HaveCount(2);
        hipoteca.MemberShares.Sum(s => s.MonthlyShare.Amount)
            .Should().BeApproximately(7_100m, precision: 1m);
    }

    [Fact]
    public void CalculateBudgetSummary_MonthlyBalance_IsIncomeMinusExpenses()
    {
        var (group, expenses) = CreateScenario();

        var summary = _service.CalculateBudgetSummary(group, expenses);

        // Income: 59,600 - Expenses: 19,150 = 40,450
        summary.MonthlyBalance.Amount.Should().BeApproximately(40_450m, precision: 1m);
        summary.HasDeficit.Should().BeFalse();
    }
}
