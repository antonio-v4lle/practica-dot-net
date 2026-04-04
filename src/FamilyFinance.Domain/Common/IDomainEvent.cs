namespace FamilyFinance.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
