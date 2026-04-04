using FamilyFinance.Domain.Common;
using FamilyFinance.Domain.ValueObjects;

namespace FamilyFinance.Domain.Events;

public record MemberAddedEvent(Guid FamilyGroupId, Guid MemberId, string Name, Money Income) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
