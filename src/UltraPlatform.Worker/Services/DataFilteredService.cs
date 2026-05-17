using Microsoft.EntityFrameworkCore;
using UltraPlatform.Worker.Database;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Models;
using UltraPlatform.Worker.Specifications;
using UltraPlatform.Worker.Specifications.Data;

namespace UltraPlatform.Worker.Services;

/// <summary>
/// Wrapper/Decorator for IDataService that extends functionality with filtering and custom logic.
/// Uses Specification pattern for composable, safe query filtering.
/// </summary>
public class DataFilteredService : IDataService
{
    private readonly IDataService _inner;
    private readonly IDataValidationHelper _helper;
    private readonly IEnumerable<ISpecification<DataRecord>> _specs;
    private readonly AppDbContext _dbContext;
    public DataFilteredService(
        IDataService inner,
        IDataValidationHelper helper,
        IEnumerable<ISpecification<DataRecord>> specs,
        AppDbContext dbContext)
    {
        _inner = inner          ?? throw new ArgumentNullException(nameof(inner));
        _specs = specs          ?? throw new ArgumentNullException(nameof(specs));
        _helper = helper        ?? throw new ArgumentNullException(nameof(helper));
        _dbContext = dbContext  ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<bool> ComposeValidation()
    {
        return await FirstDataPrivateValidationFiltered() && await _helper.SecondValidation();
    }

    private async Task<bool> FirstDataPrivateValidationFiltered()
    {
        var query = _dbContext.DataRecords.AsQueryable();

        foreach(var spec in _specs)
        {
            query = spec.Apply(query);
        }

        return await query.CountAsync<DataRecord>() > 0;
    }

    #region delegate to _inner
    // Repository methods (delegated with potential extensions)
    public IEnumerable<DataRecord> GetAll()
    {
        var records = _inner.GetAll();
        // TODO: Add filtering logic here
        return records;
    }

    public DataRecord? GetById(string id)
    {
        var record = _inner.GetById(id);
        // TODO: Add filtering logic here
        return record;
    }

    public void Add(DataRecord record)
    {
        // TODO: Add pre-validation/transformation logic here
        _inner.Add(record);
        // TODO: Add post-add logic here
    }

    public void Update(DataRecord record)
    {
        // TODO: Add pre-validation/transformation logic here
        _inner.Update(record);
        // TODO: Add post-update logic here
    }

    public void Delete(string id)
    {
        // TODO: Add pre-delete validation here
        _inner.Delete(id);
        // TODO: Add post-delete logic here
    }

    public IEnumerable<DataRecord> GetTodaysRecords()
    {
        // TODO: Add pre-getTodayRecords validation here
        var records = _inner.GetTodaysRecords();
        // TODO: Add post-getTodayRecords logic here
        return records;
    }
    #endregion

}
