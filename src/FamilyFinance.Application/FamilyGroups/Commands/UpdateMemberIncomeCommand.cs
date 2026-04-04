using FamilyFinance.Application.Common;
using FamilyFinance.Domain.Repositories;
using FamilyFinance.Domain.ValueObjects;
using MediatR;

namespace FamilyFinance.Application.FamilyGroups.Commands;

public record UpdateMemberIncomeCommand(Guid FamilyGroupId, Guid MemberId, decimal NewIncome, string Currency = "MXN") : IRequest;

public class UpdateMemberIncomeHandler(IFamilyGroupRepository repository, IUnitOfWork uow)
    : IRequestHandler<UpdateMemberIncomeCommand>
{
    public async Task Handle(UpdateMemberIncomeCommand request, CancellationToken ct)
    {
        var group = await repository.GetByIdWithMembersAsync(request.FamilyGroupId, ct)
            ?? throw new KeyNotFoundException($"Family group {request.FamilyGroupId} not found.");

        group.UpdateMemberIncome(request.MemberId, new Money(request.NewIncome, request.Currency));
        await repository.UpdateAsync(group, ct);
        await uow.SaveChangesAsync(ct);
    }
}
