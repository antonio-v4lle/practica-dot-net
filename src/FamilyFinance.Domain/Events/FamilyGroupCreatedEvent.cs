using FamilyFinance.Domain.Common;

namespace FamilyFinance.Domain.Events;

public record FamilyGroupCreatedEvent(Guid FamilyGroupId, string Name) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
