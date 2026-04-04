using FamilyFinance.Domain.Common;
using FamilyFinance.Domain.ValueObjects;

namespace FamilyFinance.Domain.Events;

public record MemberIncomeUpdatedEvent(Guid FamilyGroupId, Guid MemberId, Money NewIncome) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
