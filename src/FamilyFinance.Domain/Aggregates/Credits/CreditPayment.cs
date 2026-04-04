using FamilyFinance.Domain.Common;
using FamilyFinance.Domain.ValueObjects;

namespace FamilyFinance.Domain.Aggregates.Credits;

public class CreditPayment : Entity
{
    public Guid CreditLineId { get; private set; }
    public Money TotalAmount { get; private set; } = default!;
    public Money InterestPortion { get; private set; } = default!;
    public Money PrincipalPortion { get; private set; } = default!;
    public DateOnly PaymentDate { get; private set; }
    public string? Notes { get; private set; }

    // EF Core constructor
    private CreditPayment() { }

    internal static CreditPayment Create(
        Guid creditLineId,
        Money totalAmount,
        Money interestPortion,
        Money principalPortion,
        DateOnly paymentDate,
        string? notes = null) =>
        new()
        {
            CreditLineId = creditLineId,
            TotalAmount = totalAmount,
            InterestPortion = interestPortion,
            PrincipalPortion = principalPortion,
            PaymentDate = paymentDate,
            Notes = notes
        };
}
