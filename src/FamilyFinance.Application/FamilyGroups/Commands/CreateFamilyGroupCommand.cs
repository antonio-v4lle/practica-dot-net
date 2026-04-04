using FamilyFinance.Application.Common;
using FamilyFinance.Domain.Aggregates.FamilyBudget;
using FamilyFinance.Domain.Repositories;
using MediatR;

namespace FamilyFinance.Application.FamilyGroups.Commands;

public record CreateFamilyGroupCommand(string Name, string Currency = "MXN") : IRequest<Guid>;

public class CreateFamilyGroupHandler(IFamilyGroupRepository repository, IUnitOfWork uow)
    : IRequestHandler<CreateFamilyGroupCommand, Guid>
{
    public async Task<Guid> Handle(CreateFamilyGroupCommand request, CancellationToken ct)
    {
        var group = FamilyGroup.Create(request.Name, request.Currency);
        await repository.AddAsync(group, ct);
        await uow.SaveChangesAsync(ct);
        return group.Id;
    }
}
