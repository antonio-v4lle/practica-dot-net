using FamilyFinance.Application.Common;
using FamilyFinance.Domain.Repositories;
using FamilyFinance.Domain.ValueObjects;
using MediatR;

namespace FamilyFinance.Application.Credits.Commands;

public record UpdateCreditBalanceCommand(Guid CreditLineId, decimal NewBalance, string Currency = "MXN") : IRequest;

public class UpdateCreditBalanceHandler(ICreditLineRepository repository, IUnitOfWork uow)
    : IRequestHandler<UpdateCreditBalanceCommand>
{
    public async Task Handle(UpdateCreditBalanceCommand request, CancellationToken ct)
    {
        var credit = await repository.GetByIdAsync(request.CreditLineId, ct)
            ?? throw new KeyNotFoundException($"Credit line {request.CreditLineId} not found.");

        credit.UpdateBalance(new Money(request.NewBalance, request.Currency));
        await repository.UpdateAsync(credit, ct);
        await uow.SaveChangesAsync(ct);
    }
}
