using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Interfaces;

interface IDataRepository
{
    IEnumerable<DataRecord> GetAll();
    DataRecord? GetById(string id);
    void Add(DataRecord person);
    void Update(DataRecord person);
    void Delete(string id);

    IEnumerable<DataRecord> GetTodaysSchedules();
}