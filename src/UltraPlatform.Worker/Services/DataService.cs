using UltraPlatform.Worker.Interfaces;

namespace UltraPlatform.Worker.Services;

public class DataService : IDataService
{
    public DataService()
    {
        
    }

    public async Task<bool> PublicContractMethod()
    {
        return true;
    }
    public async Task<bool> ComposeValidation()
    {
        return await FirstDataPrivateValidation() && await SecondDataPrivateValidation();
    }

    private async Task<bool> FirstDataPrivateValidation()
    {
        return true;
    }

    private async Task<bool> SecondDataPrivateValidation()
    {
        return true;
    }
}