using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Specifications.Data;

/// <summary>
/// Specification for filtering records from today.
/// </summary>
public class TodaysRecordsSpecification : Specification
{
    public override IEnumerable<DataRecord> Apply(IEnumerable<DataRecord> query)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return query.Where(r => r.Date == today);
    }
}
