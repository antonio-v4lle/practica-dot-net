using FamilyFinance.Domain.Aggregates.FamilyBudget;
using FamilyFinance.Domain.Repositories;
using MediatR;

namespace FamilyFinance.Application.FamilyGroups.Queries;

public record GetFamilyGroupQuery(Guid FamilyGroupId) : IRequest<FamilyGroup>;

public class GetFamilyGroupHandler(IFamilyGroupRepository repository)
    : IRequestHandler<GetFamilyGroupQuery, FamilyGroup>
{
    public async Task<FamilyGroup> Handle(GetFamilyGroupQuery request, CancellationToken ct)
    {
        return await repository.GetByIdWithMembersAsync(request.FamilyGroupId, ct)
            ?? throw new KeyNotFoundException($"Family group {request.FamilyGroupId} not found.");
    }
}
