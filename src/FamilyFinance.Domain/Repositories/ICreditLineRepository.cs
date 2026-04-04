using FamilyFinance.Domain.Aggregates.Credits;

namespace FamilyFinance.Domain.Repositories;

public interface ICreditLineRepository
{
    Task<CreditLine?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CreditLine?> GetByIdWithPaymentsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CreditLine>> GetByFamilyGroupAsync(Guid familyGroupId, CancellationToken ct = default);
    Task<IReadOnlyList<CreditLine>> GetByMemberAsync(Guid memberId, CancellationToken ct = default);
    Task AddAsync(CreditLine creditLine, CancellationToken ct = default);
    Task UpdateAsync(CreditLine creditLine, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
