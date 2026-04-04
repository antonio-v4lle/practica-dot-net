using FamilyFinance.Application.Common;
using FamilyFinance.Domain.Repositories;
using FamilyFinance.Domain.ValueObjects;
using MediatR;

namespace FamilyFinance.Application.FamilyGroups.Commands;

public record AddMemberCommand(Guid FamilyGroupId, string Name, decimal MonthlyNetIncome, string Currency = "MXN") : IRequest<Guid>;

public class AddMemberHandler(IFamilyGroupRepository repository, IUnitOfWork uow)
    : IRequestHandler<AddMemberCommand, Guid>
{
    public async Task<Guid> Handle(AddMemberCommand request, CancellationToken ct)
    {
        var group = await repository.GetByIdWithMembersAsync(request.FamilyGroupId, ct)
            ?? throw new KeyNotFoundException($"Family group {request.FamilyGroupId} not found.");

        var income = new Money(request.MonthlyNetIncome, request.Currency);
        var member = group.AddMember(request.Name, income);
        await repository.UpdateAsync(group, ct);
        await uow.SaveChangesAsync(ct);
        return member.Id;
    }
}
