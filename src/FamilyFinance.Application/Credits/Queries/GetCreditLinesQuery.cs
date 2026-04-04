using FamilyFinance.Domain.Aggregates.Credits;
using FamilyFinance.Domain.Repositories;
using MediatR;

namespace FamilyFinance.Application.Credits.Queries;

public record GetCreditLinesQuery(Guid FamilyGroupId) : IRequest<IReadOnlyList<CreditLine>>;

public class GetCreditLinesHandler(ICreditLineRepository repository)
    : IRequestHandler<GetCreditLinesQuery, IReadOnlyList<CreditLine>>
{
    public async Task<IReadOnlyList<CreditLine>> Handle(GetCreditLinesQuery request, CancellationToken ct) =>
        await repository.GetByFamilyGroupAsync(request.FamilyGroupId, ct);
}
