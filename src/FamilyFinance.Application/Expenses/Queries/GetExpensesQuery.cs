using FamilyFinance.Domain.Aggregates.Expenses;
using FamilyFinance.Domain.Repositories;
using MediatR;

namespace FamilyFinance.Application.Expenses.Queries;

public record GetExpensesQuery(Guid FamilyGroupId, bool ActiveOnly = true) : IRequest<IReadOnlyList<SharedExpense>>;

public class GetExpensesHandler(IExpenseRepository repository)
    : IRequestHandler<GetExpensesQuery, IReadOnlyList<SharedExpense>>
{
    public async Task<IReadOnlyList<SharedExpense>> Handle(GetExpensesQuery request, CancellationToken ct) =>
        request.ActiveOnly
            ? await repository.GetActiveByFamilyGroupAsync(request.FamilyGroupId, ct)
            : await repository.GetByFamilyGroupAsync(request.FamilyGroupId, ct);
}
