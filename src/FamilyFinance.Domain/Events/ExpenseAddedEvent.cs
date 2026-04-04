using FamilyFinance.Domain.Common;
using FamilyFinance.Domain.ValueObjects;

namespace FamilyFinance.Domain.Events;

public record ExpenseAddedEvent(Guid ExpenseId, Guid FamilyGroupId, string Description, Money Amount) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
