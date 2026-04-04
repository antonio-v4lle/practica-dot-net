using FamilyFinance.Domain.Repositories;
using FamilyFinance.Domain.Services;
using MediatR;

namespace FamilyFinance.Application.FamilyGroups.Queries;

public record GetBudgetSummaryQuery(Guid FamilyGroupId) : IRequest<BudgetSummary>;

public class GetBudgetSummaryHandler(
    IFamilyGroupRepository groupRepo,
    IExpenseRepository expenseRepo,
    ProportionalContributionService contributionService)
    : IRequestHandler<GetBudgetSummaryQuery, BudgetSummary>
{
    public async Task<BudgetSummary> Handle(GetBudgetSummaryQuery request, CancellationToken ct)
    {
        var group = await groupRepo.GetByIdWithMembersAsync(request.FamilyGroupId, ct)
            ?? throw new KeyNotFoundException($"Family group {request.FamilyGroupId} not found.");

        var expenses = await expenseRepo.GetActiveByFamilyGroupAsync(request.FamilyGroupId, ct);
        return contributionService.CalculateBudgetSummary(group, expenses);
    }
}
