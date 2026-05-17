using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Specifications.Data;

/// <summary>
/// Generic specification for filtering records from today.
/// Can be reused with any type that has a Date property.
/// </summary>
public class TodaysRecordsSpecification : Specification<DataRecord>
{
    public override IQueryable<DataRecord> Apply(IQueryable<DataRecord> query)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return query.Where(r => r.Date == today);
    }
}
