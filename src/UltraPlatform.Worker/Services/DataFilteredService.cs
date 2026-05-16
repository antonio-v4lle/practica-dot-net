
using UltraPlatform.Worker.Interfaces;

namespace UltraPlatform.Worker.Services;

public class DataFilteredService : IDataService
{
    private IDataService _inner;

    public DataFilteredService(IDataService dataService)
    {
        _inner = dataService ?? throw new ArgumentNullException(nameof(dataService));
    }
    
    public async Task<bool> PublicContractMethod()
    {
        return await _inner.PublicContractMethod();
    }

    public Task<bool> ComposeValidation()
    {
        // Custom Validations
        throw new NotImplementedException();
    }
}