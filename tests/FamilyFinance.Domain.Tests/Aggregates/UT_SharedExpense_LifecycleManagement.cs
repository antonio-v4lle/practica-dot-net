using FamilyFinance.Domain.Aggregates.Expenses;
using FamilyFinance.Domain.ValueObjects;
using FluentAssertions;

namespace FamilyFinance.Domain.Tests.Aggregates;

/// <summary>UT_SharedExpense_LifecycleManagement — ciclo de vida: creación, suspensión, reactivación.</summary>
public class UT_SharedExpense_LifecycleManagement
{
    private static readonly Guid FamilyId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_CreatesActiveExpense()
    {
        var expense = SharedExpense.Create(FamilyId, "Hipoteca", ExpenseCategory.Patrimonio,
            Money.Of(7_100m), isRecurring: true, paymentDay: 19);

        expense.IsActive.Should().BeTrue();
        expense.MonthlyAmount.Amount.Should().Be(7_100m);
        expense.Schedule!.PaymentDay.Should().Be(19);
        expense.Category.Should().Be(ExpenseCategory.Patrimonio);
        expense.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "ExpenseAddedEvent");
    }

    [Fact]
    public void Create_WithEmptyDescription_ThrowsArgumentException()
    {
        var act = () => SharedExpense.Create(FamilyId, " ", ExpenseCategory.Servicios,
            Money.Of(1_000m));

        act.Should().Throw<ArgumentException>().WithMessage("*description*");
    }

    [Fact]
    public void Create_WithNegativeAmount_ThrowsArgumentException()
    {
        var act = () => SharedExpense.Create(FamilyId, "Test", ExpenseCategory.Servicios,
            new Money(-100m));

        act.Should().Throw<ArgumentException>().WithMessage("*negative*");
    }

    [Fact]
    public void Create_WithZeroAmount_AllowedForSuspendedExpenses()
    {
        // Expenses like Leonel/Boda can be created with 0 and marked suspended
        var act = () => SharedExpense.Create(FamilyId, "Leonel", ExpenseCategory.Educacion,
            Money.Zero, isRecurring: false);

        act.Should().NotThrow();
    }

    [Fact]
    public void FirstAndSecondFortnightAmounts_AreHalfOfMonthly()
    {
        var expense = SharedExpense.Create(FamilyId, "Hipoteca", ExpenseCategory.Patrimonio,
            Money.Of(7_100m), paymentDay: 19);

        expense.FirstFortnightAmount.Amount.Should().Be(3_550m);
        expense.SecondFortnightAmount.Amount.Should().Be(3_550m);
    }

    [Fact]
    public void Suspend_ActiveExpense_SetsAmountToZeroAndDeactivates()
    {
        var expense = SharedExpense.Create(FamilyId, "Hipoteca", ExpenseCategory.Patrimonio,
            Money.Of(7_100m));

        expense.Suspend();

        expense.IsActive.Should().BeFalse();
        expense.MonthlyAmount.IsZero.Should().BeTrue();
    }

    [Fact]
    public void Resume_SuspendedExpense_ReactivatesWithNewAmount()
    {
        var expense = SharedExpense.Create(FamilyId, "Hipoteca", ExpenseCategory.Patrimonio,
            Money.Of(7_100m));
        expense.Suspend();

        expense.Resume(Money.Of(7_500m));

        expense.IsActive.Should().BeTrue();
        expense.MonthlyAmount.Amount.Should().Be(7_500m);
    }

    [Fact]
    public void UpdateAmount_ToPositiveValue_UpdatesMonthlyAmount()
    {
        var expense = SharedExpense.Create(FamilyId, "Internet", ExpenseCategory.Servicios,
            Money.Of(1_500m));

        expense.UpdateAmount(Money.Of(1_800m));

        expense.MonthlyAmount.Amount.Should().Be(1_800m);
    }
}
