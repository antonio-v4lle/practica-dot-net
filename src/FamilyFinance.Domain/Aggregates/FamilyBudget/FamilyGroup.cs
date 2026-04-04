using FamilyFinance.Domain.Common;
using FamilyFinance.Domain.Events;
using FamilyFinance.Domain.ValueObjects;

namespace FamilyFinance.Domain.Aggregates.FamilyBudget;

public class FamilyGroup : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public string Currency { get; private set; } = "MXN";

    private readonly List<Member> _members = [];
    public IReadOnlyList<Member> Members => _members.AsReadOnly();

    // EF Core constructor
    private FamilyGroup() { }

    public static FamilyGroup Create(string name, string currency = "MXN")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Family group name cannot be empty.", nameof(name));

        var group = new FamilyGroup { Name = name.Trim(), Currency = currency };
        group.AddDomainEvent(new FamilyGroupCreatedEvent(group.Id, name));
        return group;
    }

    public Member AddMember(string name, Money monthlyNetIncome)
    {
        if (_members.Any(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A member named '{name}' already exists.");

        var member = Member.Create(Id, name, monthlyNetIncome);
        _members.Add(member);
        RecalculateProportions();
        AddDomainEvent(new MemberAddedEvent(Id, member.Id, name, monthlyNetIncome));
        return member;
    }

    public void UpdateMemberIncome(Guid memberId, Money newIncome)
    {
        var member = GetMemberOrThrow(memberId);
        member.UpdateIncome(newIncome);
        RecalculateProportions();
        AddDomainEvent(new MemberIncomeUpdatedEvent(Id, memberId, newIncome));
    }

    public Money TotalMonthlyIncome =>
        _members.Aggregate(Money.Zero, (acc, m) => acc.Add(m.MonthlyNetIncome));

    public Money TotalBiweeklyIncome =>
        TotalMonthlyIncome.Divide(2);

    /// <summary>Returns the deficit: what each member must contribute beyond their minimum.</summary>
    public Money CalculateMonthlyDeficit(Money totalSharedExpenses)
    {
        var income = TotalMonthlyIncome;
        return income.Amount > totalSharedExpenses.Amount
            ? Money.Zero
            : totalSharedExpenses.Subtract(income);
    }

    private void RecalculateProportions()
    {
        var total = TotalMonthlyIncome;
        foreach (var member in _members)
            member.SetProportion(ProportionRatio.Calculate(member.MonthlyNetIncome, total));
    }

    private Member GetMemberOrThrow(Guid memberId) =>
        _members.FirstOrDefault(m => m.Id == memberId)
        ?? throw new InvalidOperationException($"Member {memberId} not found in family group.");
}
