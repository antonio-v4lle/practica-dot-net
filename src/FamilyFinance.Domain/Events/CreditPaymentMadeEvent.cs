using FamilyFinance.Domain.Common;
using FamilyFinance.Domain.ValueObjects;

namespace FamilyFinance.Domain.Events;

public record CreditPaymentMadeEvent(
    Guid CreditLineId,
    Guid FamilyGroupId,
    Guid MemberId,
    Money PaymentAmount,
    Money RemainingBalance) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
