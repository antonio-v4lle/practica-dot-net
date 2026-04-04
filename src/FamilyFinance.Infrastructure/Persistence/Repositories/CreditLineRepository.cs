using FamilyFinance.Domain.Aggregates.Credits;
using FamilyFinance.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FamilyFinance.Infrastructure.Persistence.Repositories;

public class CreditLineRepository(FamilyFinanceDbContext context) : ICreditLineRepository
{
    public async Task<CreditLine?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.CreditLines.FindAsync([id], ct);

    public async Task<CreditLine?> GetByIdWithPaymentsAsync(Guid id, CancellationToken ct = default) =>
        await context.CreditLines
            .Include(c => c.Payments)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<CreditLine>> GetByFamilyGroupAsync(Guid familyGroupId, CancellationToken ct = default) =>
        await context.CreditLines
            .Where(c => c.FamilyGroupId == familyGroupId && c.IsActive)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CreditLine>> GetByMemberAsync(Guid memberId, CancellationToken ct = default) =>
        await context.CreditLines
            .Where(c => c.MemberId == memberId && c.IsActive)
            .ToListAsync(ct);

    public async Task AddAsync(CreditLine creditLine, CancellationToken ct = default) =>
        await context.CreditLines.AddAsync(creditLine, ct);

    public Task UpdateAsync(CreditLine creditLine, CancellationToken ct = default)
    {
        // Already tracked by EF Core change tracker.
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var credit = await GetByIdAsync(id, ct);
        if (credit is not null) context.CreditLines.Remove(credit);
    }
}
