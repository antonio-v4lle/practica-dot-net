using FamilyFinance.Application.Common;
using FamilyFinance.Domain.Repositories;
using MediatR;

namespace FamilyFinance.Application.Expenses.Commands;

public record SuspendExpenseCommand(Guid ExpenseId) : IRequest;

public class SuspendExpenseHandler(IExpenseRepository repository, IUnitOfWork uow)
    : IRequestHandler<SuspendExpenseCommand>
{
    public async Task Handle(SuspendExpenseCommand request, CancellationToken ct)
    {
        var expense = await repository.GetByIdAsync(request.ExpenseId, ct)
            ?? throw new KeyNotFoundException($"Expense {request.ExpenseId} not found.");

        expense.Suspend();
        await repository.UpdateAsync(expense, ct);
        await uow.SaveChangesAsync(ct);
    }
}
