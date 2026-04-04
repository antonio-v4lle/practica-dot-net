using FamilyFinance.Domain.Aggregates.Expenses;

namespace FamilyFinance.Domain.Repositories;

public interface IExpenseRepository
{
    Task<SharedExpense?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SharedExpense>> GetByFamilyGroupAsync(Guid familyGroupId, CancellationToken ct = default);
    Task<IReadOnlyList<SharedExpense>> GetActiveByFamilyGroupAsync(Guid familyGroupId, CancellationToken ct = default);
    Task AddAsync(SharedExpense expense, CancellationToken ct = default);
    Task UpdateAsync(SharedExpense expense, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
