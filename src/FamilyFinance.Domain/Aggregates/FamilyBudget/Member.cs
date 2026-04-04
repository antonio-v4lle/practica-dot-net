using FamilyFinance.Domain.Common;
using FamilyFinance.Domain.ValueObjects;

namespace FamilyFinance.Domain.Aggregates.FamilyBudget;

public class Member : Entity
{
    public Guid FamilyGroupId { get; private set; }
    public string Name { get; private set; } = default!;
    public Money MonthlyNetIncome { get; private set; } = default!;
    public ProportionRatio Proportion { get; private set; } = default!;

    // EF Core constructor
    private Member() { }

    internal static Member Create(Guid familyGroupId, string name, Money monthlyNetIncome)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Member name cannot be empty.", nameof(name));
        if (!monthlyNetIncome.IsPositive)
            throw new ArgumentException("Monthly income must be positive.", nameof(monthlyNetIncome));

        return new Member
        {
            FamilyGroupId = familyGroupId,
            Name = name.Trim(),
            MonthlyNetIncome = monthlyNetIncome,
            Proportion = ProportionRatio.From(0) // Recalculated at family group level
        };
    }

    public void UpdateIncome(Money newIncome)
    {
        if (!newIncome.IsPositive)
            throw new ArgumentException("Monthly income must be positive.", nameof(newIncome));
        MonthlyNetIncome = newIncome;
    }

    internal void SetProportion(ProportionRatio proportion) => Proportion = proportion;

    public Money CalculateContribution(Money sharedExpense) =>
        sharedExpense.Multiply(Proportion.Value);

    public Money BiweeklyContribution(Money sharedExpense) =>
        CalculateContribution(sharedExpense).Divide(2);
}
