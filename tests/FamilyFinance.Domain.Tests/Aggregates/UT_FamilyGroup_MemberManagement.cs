using FamilyFinance.Domain.Aggregates.FamilyBudget;
using FamilyFinance.Domain.ValueObjects;
using FluentAssertions;

namespace FamilyFinance.Domain.Tests.Aggregates;

/// <summary>UT_FamilyGroup_MemberManagement — comportamiento del agregado raíz al gestionar miembros.</summary>
public class UT_FamilyGroup_MemberManagement
{
    [Fact]
    public void Create_WithValidName_CreatesGroupAndRaisesDomainEvent()
    {
        var group = FamilyGroup.Create("Familia García");

        group.Name.Should().Be("Familia García");
        group.Currency.Should().Be("MXN");
        group.Members.Should().BeEmpty();
        group.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "FamilyGroupCreatedEvent");
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        var act = () => FamilyGroup.Create("  ");

        act.Should().Throw<ArgumentException>().WithMessage("*name*");
    }

    [Fact]
    public void AddMember_FirstMember_Gets100PercentProportion()
    {
        var group = FamilyGroup.Create("Test");

        group.AddMember("Antonio", Money.Of(42_000m));

        group.Members.Should().HaveCount(1);
        group.Members[0].Proportion.Value.Should().Be(1m);
    }

    [Fact]
    public void AddMember_TwoMembers_RecalculatesProportionsCorrectly()
    {
        var group = FamilyGroup.Create("Test");
        group.AddMember("Antonio", Money.Of(42_000m));
        group.AddMember("Adriana", Money.Of(17_600m));

        var antonio = group.Members.First(m => m.Name == "Antonio");
        var adriana = group.Members.First(m => m.Name == "Adriana");

        antonio.Proportion.Percentage.Should().BeApproximately(70.47m, 0.01m);
        adriana.Proportion.Percentage.Should().BeApproximately(29.53m, 0.01m);
        (antonio.Proportion.Value + adriana.Proportion.Value).Should().BeApproximately(1m, 0.0001m);
    }

    [Fact]
    public void AddMember_DuplicateName_ThrowsInvalidOperationException()
    {
        var group = FamilyGroup.Create("Test");
        group.AddMember("Antonio", Money.Of(42_000m));

        var act = () => group.AddMember("Antonio", Money.Of(10_000m));

        act.Should().Throw<InvalidOperationException>().WithMessage("*Antonio*");
    }

    [Fact]
    public void AddMember_WithZeroIncome_ThrowsArgumentException()
    {
        var group = FamilyGroup.Create("Test");

        var act = () => group.AddMember("Juan", Money.Zero);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateMemberIncome_ChangesIncomeAndRecalculatesProportions()
    {
        var group = FamilyGroup.Create("Test");
        group.AddMember("Antonio", Money.Of(42_000m));
        group.AddMember("Adriana", Money.Of(17_600m));
        var antonio = group.Members.First(m => m.Name == "Antonio");

        group.UpdateMemberIncome(antonio.Id, Money.Of(50_000m));

        antonio.MonthlyNetIncome.Amount.Should().Be(50_000m);
        // New proportions: 50k / 67.6k ≈ 73.96%, Adriana ≈ 26.04%
        antonio.Proportion.Percentage.Should().BeApproximately(73.96m, 0.01m);
    }

    [Fact]
    public void UpdateMemberIncome_UnknownMemberId_ThrowsInvalidOperationException()
    {
        var group = FamilyGroup.Create("Test");
        group.AddMember("Antonio", Money.Of(42_000m));

        var act = () => group.UpdateMemberIncome(Guid.NewGuid(), Money.Of(50_000m));

        act.Should().Throw<InvalidOperationException>().WithMessage("*not found*");
    }

    [Fact]
    public void TotalMonthlyIncome_TwoMembers_ReturnsCombinedIncome()
    {
        var group = FamilyGroup.Create("Test");
        group.AddMember("Antonio", Money.Of(42_000m));
        group.AddMember("Adriana", Money.Of(17_600m));

        group.TotalMonthlyIncome.Amount.Should().Be(59_600m);
        group.TotalBiweeklyIncome.Amount.Should().Be(29_800m);
    }

    [Fact]
    public void AddMember_SetsCorrectFamilyGroupId()
    {
        var group = FamilyGroup.Create("Test");

        var member = group.AddMember("Antonio", Money.Of(42_000m));

        member.FamilyGroupId.Should().Be(group.Id);
    }
}
