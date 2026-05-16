using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Interfaces;

public interface IDataService
{
    // Repository methods
    IEnumerable<DataRecord> GetAll();
    DataRecord? GetById(string id);
    void Add(DataRecord record);
    void Update(DataRecord record);
    void Delete(string id);
    IEnumerable<DataRecord> GetTodaysRecords();

    // Business logic methods
    Task<bool> ComposeValidation();
    Task<bool> PublicContractMethod();
}
