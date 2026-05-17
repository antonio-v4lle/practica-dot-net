using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Services;

public class DataService : IDataService
{
    private readonly List<DataRecord> _data = new();
    private readonly IDataValidationHelper _helper;

    public DataService(IDataValidationHelper helper)
    {
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
    }

    // Repository methods (consolidated)
    public IEnumerable<DataRecord> GetAll() => _data.ToList();

    public DataRecord? GetById(string id) => _data.FirstOrDefault(d => d.Id == id);

    public void Add(DataRecord record)
    {
        if (_data.Any(d => d.Id == record.Id))
            throw new InvalidOperationException($"Record with id {record.Id} already exists.");
        _data.Add(record);
    }

    public void Update(DataRecord record)
    {
        var existing = _data.FirstOrDefault(d => d.Id == record.Id);
        if (existing == null)
            throw new InvalidOperationException($"Record with id {record.Id} not found.");
        
        int index = _data.IndexOf(existing);
        _data[index] = record;
    }

    public void Delete(string id)
    {
        var record = _data.FirstOrDefault(d => d.Id == id);
        if (record == null)
            throw new InvalidOperationException($"Record with id {id} not found.");
        _data.Remove(record);
    }

    public IEnumerable<DataRecord> GetTodaysRecords()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return _data.Where(d => d.Date == today).ToList();
    }

    // Business logic methods
    public async Task<bool> ComposeValidation()
    {
        return await FirstDataPrivateValidation() && await SecondDataPrivateValidation();
    }

    /// <summary>
    /// Returns bool 6 lines of logic without filtering
    /// Wrapper can apply additional specifications if needed.
    /// </summary>
    private async Task<bool> FirstDataPrivateValidation()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        // Simplification of logic
        var arbitraryValidation = _data.Where(d => d.Date == today).ToList().Count() % 2;
        // After 6 lines and other queries for others Repositories.
        return arbitraryValidation == 0;
    }

    private async Task<bool> SecondDataPrivateValidation() => await _helper.SecondValidation();
}
