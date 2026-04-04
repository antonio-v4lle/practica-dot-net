using FamilyFinance.Application.Common;
using FamilyFinance.Domain.Aggregates.Credits;
using FamilyFinance.Domain.Repositories;
using FamilyFinance.Domain.ValueObjects;
using MediatR;

namespace FamilyFinance.Application.Credits.Commands;

public record MakeCreditPaymentCommand(
    Guid CreditLineId,
    decimal Amount,
    DateOnly PaymentDate,
    string Currency = "MXN",
    string? Notes = null) : IRequest<Guid>;

public class MakeCreditPaymentHandler(ICreditLineRepository repository, IUnitOfWork uow)
    : IRequestHandler<MakeCreditPaymentCommand, Guid>
{
    public async Task<Guid> Handle(MakeCreditPaymentCommand request, CancellationToken ct)
    {
        var credit = await repository.GetByIdAsync(request.CreditLineId, ct)
            ?? throw new KeyNotFoundException($"Credit line {request.CreditLineId} not found.");

        var payment = credit.MakePayment(
            new Money(request.Amount, request.Currency),
            request.PaymentDate,
            request.Notes);

        await repository.UpdateAsync(credit, ct);
        await uow.SaveChangesAsync(ct);
        return payment.Id;
    }
}
