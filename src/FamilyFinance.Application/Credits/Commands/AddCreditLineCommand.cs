using FamilyFinance.Application.Common;
using FamilyFinance.Domain.Aggregates.Credits;
using FamilyFinance.Domain.Repositories;
using FamilyFinance.Domain.ValueObjects;
using MediatR;

namespace FamilyFinance.Application.Credits.Commands;

public record AddCreditLineCommand(
    Guid FamilyGroupId,
    Guid MemberId,
    string Name,
    decimal CreditLimit,
    decimal CurrentBalance,
    decimal AnnualInterestRate,
    int PaymentDueDay,
    decimal? MinimumPayment = null,
    string Currency = "MXN") : IRequest<Guid>;

public class AddCreditLineHandler(ICreditLineRepository repository, IUnitOfWork uow)
    : IRequestHandler<AddCreditLineCommand, Guid>
{
    public async Task<Guid> Handle(AddCreditLineCommand request, CancellationToken ct)
    {
        var creditLine = CreditLine.Create(
            request.FamilyGroupId,
            request.MemberId,
            request.Name,
            new Money(request.CreditLimit, request.Currency),
            new Money(request.CurrentBalance, request.Currency),
            InterestRate.From(request.AnnualInterestRate),
            request.PaymentDueDay,
            request.MinimumPayment.HasValue ? new Money(request.MinimumPayment.Value, request.Currency) : null);

        await repository.AddAsync(creditLine, ct);
        await uow.SaveChangesAsync(ct);
        return creditLine.Id;
    }
}
