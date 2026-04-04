using FamilyFinance.Application.Common;
using FamilyFinance.Domain.Aggregates.Expenses;
using FamilyFinance.Domain.Repositories;
using FamilyFinance.Domain.ValueObjects;
using MediatR;

namespace FamilyFinance.Application.Expenses.Commands;

public record UpdateExpenseCommand(Guid ExpenseId, decimal? NewAmount, string? NewDescription, string Currency = "MXN") : IRequest;

public class UpdateExpenseHandler(IExpenseRepository repository, IUnitOfWork uow)
    : IRequestHandler<UpdateExpenseCommand>
{
    public async Task Handle(UpdateExpenseCommand request, CancellationToken ct)
    {
        var expense = await repository.GetByIdAsync(request.ExpenseId, ct)
            ?? throw new KeyNotFoundException($"Expense {request.ExpenseId} not found.");

        if (request.NewAmount.HasValue)
            expense.UpdateAmount(new Money(request.NewAmount.Value, request.Currency));
        if (request.NewDescription is not null)
            expense.UpdateDescription(request.NewDescription);

        await repository.UpdateAsync(expense, ct);
        await uow.SaveChangesAsync(ct);
    }
}
