using FamilyFinance.Domain.Aggregates.FamilyBudget;
using FamilyFinance.Domain.ValueObjects;
using FluentAssertions;

namespace FamilyFinance.Domain.Tests.Aggregates;

/// <summary>UT_Member_ContributionCalculation — cálculo de aportación proporcional por miembro.</summary>
public class UT_Member_ContributionCalculation
{
    private static FamilyGroup CreateFamilyWithTwoMembers()
    {
        var group = FamilyGroup.Create("Test");
        group.AddMember("Antonio", Money.Of(42_000m));
        group.AddMember("Adriana", Money.Of(17_600m));
        return group;
    }

    [Fact]
    public void CalculateContribution_Antonio_PaysProportionalShare()
    {
        var group = CreateFamilyWithTwoMembers();
        var antonio = group.Members.First(m => m.Name == "Antonio");
        var totalExpenses = Money.Of(48_450m);

        var contribution = antonio.CalculateContribution(totalExpenses);

        // 48,450 × (42000/59600) ≈ 34,142.72
        contribution.Amount.Should().BeApproximately(34_142.72m, precision: 1m);
    }

    [Fact]
    public void BiweeklyContribution_IsHalfOfMonthlyContribution()
    {
        var group = CreateFamilyWithTwoMembers();
        var antonio = group.Members.First(m => m.Name == "Antonio");
        var totalExpenses = Money.Of(48_450m);

        var monthly = antonio.CalculateContribution(totalExpenses);
        var biweekly = antonio.BiweeklyContribution(totalExpenses);

        biweekly.Amount.Should().BeApproximately(monthly.Amount / 2, precision: 1m);
    }

    [Fact]
    public void CalculateContribution_BothMembers_SumEqualsTotal()
    {
        var group = CreateFamilyWithTwoMembers();
        var totalExpenses = Money.Of(48_450m);

        var sum = group.Members
            .Select(m => m.CalculateContribution(totalExpenses).Amount)
            .Sum();

        sum.Should().BeApproximately(totalExpenses.Amount, precision: 1m);
    }
}
