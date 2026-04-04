using FamilyFinance.Domain.Aggregates.FamilyBudget;

namespace FamilyFinance.Domain.Repositories;

public interface IFamilyGroupRepository
{
    Task<FamilyGroup?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FamilyGroup?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<FamilyGroup>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(FamilyGroup group, CancellationToken ct = default);
    Task UpdateAsync(FamilyGroup group, CancellationToken ct = default);
}
