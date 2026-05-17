using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Specifications.Data;

/// <summary>
/// Generic specification that excludes records with specific codes (e.g., test/inactive codes).
/// </summary>
public class ExcludeInactiveCodesSpecification : Specification<DataRecord>
{
    private readonly HashSet<string> _excludedCodes;

    public ExcludeInactiveCodesSpecification(params string[] excludedCodes) => _excludedCodes = new HashSet<string>(excludedCodes ?? Array.Empty<string>());

    public override IQueryable<DataRecord> Apply(IQueryable<DataRecord> query)
    {
        return query.Where(r => !_excludedCodes.Contains(r.Code));
    }
}
