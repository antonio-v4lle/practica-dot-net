using FamilyFinance.Application.Common;
using FamilyFinance.Domain.Aggregates.Expenses;
using FamilyFinance.Domain.Repositories;
using FamilyFinance.Domain.ValueObjects;
using MediatR;

namespace FamilyFinance.Application.Expenses.Commands;

public record AddExpenseCommand(
    Guid FamilyGroupId,
    string Description,
    ExpenseCategory Category,
    decimal MonthlyAmount,
    bool IsRecurring = true,
    int? PaymentDay = null,
    string Currency = "MXN",
    string? Notes = null) : IRequest<Guid>;

public class AddExpenseHandler(IExpenseRepository repository, IUnitOfWork uow)
    : IRequestHandler<AddExpenseCommand, Guid>
{
    public async Task<Guid> Handle(AddExpenseCommand request, CancellationToken ct)
    {
        var expense = SharedExpense.Create(
            request.FamilyGroupId,
            request.Description,
            request.Category,
            new Money(request.MonthlyAmount, request.Currency),
            request.IsRecurring,
            request.PaymentDay,
            request.Notes);

        await repository.AddAsync(expense, ct);
        await uow.SaveChangesAsync(ct);
        return expense.Id;
    }
}
