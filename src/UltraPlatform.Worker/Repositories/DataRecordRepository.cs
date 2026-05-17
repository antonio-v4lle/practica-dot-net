using UltraPlatform.Worker.Database;
using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Repositories;

public class DataRecordRepository : IDataRecordRepository
{
    private readonly AppDbContext _dbContext;

    public DataRecordRepository(AppDbContext dbContext)
        => _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    /// <summary>
    /// Returns IQueryable of codes where IsActive = false.
    /// EF Core composes this as a subquery when used inside a spec.
    /// </summary>
    public IQueryable<string> GetInactiveCodesQueryable()
        => _dbContext.DataRecords
            .Where(r => !r.IsActive)
            .Select(r => r.Code);
}
