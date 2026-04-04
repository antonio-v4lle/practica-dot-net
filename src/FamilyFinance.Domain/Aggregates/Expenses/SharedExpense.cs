using FamilyFinance.Domain.Common;
using FamilyFinance.Domain.Events;
using FamilyFinance.Domain.ValueObjects;

namespace FamilyFinance.Domain.Aggregates.Expenses;

public class SharedExpense : AggregateRoot
{
    public Guid FamilyGroupId { get; private set; }
    public string Description { get; private set; } = default!;
    public ExpenseCategory Category { get; private set; }
    public Money MonthlyAmount { get; private set; } = default!;
    public BiweeklySchedule? Schedule { get; private set; }
    public bool IsRecurring { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Notes { get; private set; }

    public Money FirstFortnightAmount => MonthlyAmount.Divide(2);
    public Money SecondFortnightAmount => MonthlyAmount.Divide(2);

    // EF Core constructor
    private SharedExpense() { }

    public static SharedExpense Create(
        Guid familyGroupId,
        string description,
        ExpenseCategory category,
        Money monthlyAmount,
        bool isRecurring = true,
        int? paymentDay = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Expense description cannot be empty.", nameof(description));
        if (monthlyAmount.IsNegative)
            throw new ArgumentException("Expense amount cannot be negative.", nameof(monthlyAmount));

        var expense = new SharedExpense
        {
            FamilyGroupId = familyGroupId,
            Description = description.Trim(),
            Category = category,
            MonthlyAmount = monthlyAmount,
            IsRecurring = isRecurring,
            Schedule = paymentDay.HasValue ? BiweeklySchedule.OnDay(paymentDay.Value) : null,
            Notes = notes
        };

        expense.AddDomainEvent(new ExpenseAddedEvent(expense.Id, familyGroupId, description, monthlyAmount));
        return expense;
    }

    public void UpdateAmount(Money newAmount)
    {
        if (newAmount.IsNegative)
            throw new ArgumentException("Expense amount cannot be negative.", nameof(newAmount));
        MonthlyAmount = newAmount;
    }

    public void Suspend()
    {
        IsActive = false;
        MonthlyAmount = Money.Zero;
    }

    public void Resume(Money amount)
    {
        IsActive = true;
        MonthlyAmount = amount;
    }

    public void UpdateDescription(string description) =>
        Description = description.Trim();
}
