using FamilyFinance.Domain.Aggregates.FamilyBudget;
using FamilyFinance.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FamilyFinance.Infrastructure.Persistence.Repositories;

public class FamilyGroupRepository(FamilyFinanceDbContext context) : IFamilyGroupRepository
{
    public async Task<FamilyGroup?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.FamilyGroups.FindAsync([id], ct);

    public async Task<FamilyGroup?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default) =>
        await context.FamilyGroups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<IReadOnlyList<FamilyGroup>> GetAllAsync(CancellationToken ct = default) =>
        await context.FamilyGroups
            .Include(g => g.Members)
            .ToListAsync(ct);

    public async Task AddAsync(FamilyGroup group, CancellationToken ct = default) =>
        await context.FamilyGroups.AddAsync(group, ct);

    public async Task UpdateAsync(FamilyGroup group, CancellationToken ct = default)
    {
        // Disable auto-detection so that context.Entry() doesn't trigger change detection,
        // which would incorrectly mark new Guid-keyed members as Modified instead of Added.
        var prevAutoDetect = context.ChangeTracker.AutoDetectChangesEnabled;
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            foreach (var member in group.Members)
            {
                if (context.Entry(member).State == EntityState.Detached)
                    await context.Members.AddAsync(member, ct);
            }
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = prevAutoDetect;
        }
    }
}
