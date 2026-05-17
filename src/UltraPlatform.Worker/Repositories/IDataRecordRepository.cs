using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Repositories;

public interface IDataRecordRepository
{
    /// <summary>
    /// Returns IQueryable of inactive codes — EF Core translates as subquery.
    /// </summary>
    IQueryable<string> GetInactiveCodesQueryable();
}
