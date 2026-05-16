using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Specifications.Data;

/// <summary>
/// Specification that excludes records with specific codes (e.g., test/inactive codes).
/// </summary>
public class ExcludeInactiveCodesSpecification : Specification
{
    private readonly HashSet<string> _excludedCodes;

    public ExcludeInactiveCodesSpecification(params string[] excludedCodes)
    {
        _excludedCodes = new HashSet<string>(excludedCodes ?? Array.Empty<string>());
    }

    public override IEnumerable<DataRecord> Apply(IEnumerable<DataRecord> query)
    {
        return query.Where(r => !_excludedCodes.Contains(r.Code));
    }
}
