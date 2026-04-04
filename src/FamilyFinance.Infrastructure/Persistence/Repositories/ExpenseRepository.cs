using FamilyFinance.Domain.Aggregates.Expenses;
using FamilyFinance.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FamilyFinance.Infrastructure.Persistence.Repositories;

public class ExpenseRepository(FamilyFinanceDbContext context) : IExpenseRepository
{
    public async Task<SharedExpense?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.SharedExpenses.FindAsync([id], ct);

    public async Task<IReadOnlyList<SharedExpense>> GetByFamilyGroupAsync(Guid familyGroupId, CancellationToken ct = default) =>
        await context.SharedExpenses
            .Where(e => e.FamilyGroupId == familyGroupId)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<SharedExpense>> GetActiveByFamilyGroupAsync(Guid familyGroupId, CancellationToken ct = default) =>
        await context.SharedExpenses
            .Where(e => e.FamilyGroupId == familyGroupId && e.IsActive)
            .ToListAsync(ct);

    public async Task AddAsync(SharedExpense expense, CancellationToken ct = default) =>
        await context.SharedExpenses.AddAsync(expense, ct);

    public Task UpdateAsync(SharedExpense expense, CancellationToken ct = default)
    {
        // Already tracked by EF Core change tracker.
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var expense = await GetByIdAsync(id, ct);
        if (expense is not null) context.SharedExpenses.Remove(expense);
    }
}
