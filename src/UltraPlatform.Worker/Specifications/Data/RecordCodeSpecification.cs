using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Specifications.Data;

/// <summary>
/// Generic specification for filtering records by code pattern.
/// </summary>
public class RecordCodeSpecification : Specification<DataRecord>
{
    private readonly string _codePattern;
    private readonly bool _exclude;

    public RecordCodeSpecification(string codePattern, bool exclude = false)
    {
        _codePattern = codePattern ?? throw new ArgumentNullException(nameof(codePattern));
        _exclude = exclude;
    }

    public override IEnumerable<DataRecord> Apply(IEnumerable<DataRecord> query)
    {
        if (_exclude)
            return query.Where(r => !r.Code.Contains(_codePattern));
        
        return query.Where(r => r.Code.Contains(_codePattern));
    }
}
