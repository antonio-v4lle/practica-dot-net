namespace UltraPlatform.Worker.Services;

public class DataFilteredService
{
    private readonly DataService _inner;

    public DataFilteredService(DataService dataService)
    {
        _inner = dataService ?? throw new ArgumentNullException(nameof(dataService));
    }
    
    public async Task<bool> PublicContractMethod()
    {
        return await _inner.PublicContractMethod();
    }

    public async Task<bool> ComposeValidation()
    {
        // Custom Validations
        return await _inner.ComposeValidation();
    }
}
