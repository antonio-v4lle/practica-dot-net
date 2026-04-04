using FamilyFinance.Application.Common;

namespace FamilyFinance.Infrastructure.Persistence;

public class UnitOfWork(FamilyFinanceDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);
}
