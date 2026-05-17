using UltraPlatform.Worker.Models;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Specifications;

namespace UltraPlatform.Worker.Specifications.Data;

/// <summary>
/// Filters records matching today's date.
/// Depends on IUtilityService for testable date resolution.
/// </summary>
public class TodaysRecordsSpecification : Specification<DataRecord>
{
    private readonly IUtilityService _utility;

    public TodaysRecordsSpecification(IUtilityService utility)
        => _utility = utility ?? throw new ArgumentNullException(nameof(utility));

    public override IQueryable<DataRecord> Apply(IQueryable<DataRecord> query)
    {
        var today = _utility.GetToday();
        return query.Where(r => r.Date == today);
    }
}
