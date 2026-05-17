using UltraPlatform.Worker.Models;
using UltraPlatform.Worker.Repositories;
using UltraPlatform.Worker.Specifications;

namespace UltraPlatform.Worker.Specifications.Data;

/// <summary>
/// Excludes records whose Code is in the inactive codes list from DB.
/// Self-sufficient: loads its own data via IDataRecordRepository.
/// </summary>
public class ExcludeInactiveCodesSpecification : Specification<DataRecord>
{
    private readonly IDataRecordRepository _repository;

    public ExcludeInactiveCodesSpecification(IDataRecordRepository repository)
        => _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public override IQueryable<DataRecord> Apply(IQueryable<DataRecord> query)
    {
        // Loads inactive codes from DB — executes as subquery in EF Core
        var inactiveCodes = _repository.GetInactiveCodesQueryable();
        return query.Where(r => !inactiveCodes.Contains(r.Code));
    }
}
