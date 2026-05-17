using UltraPlatform.Worker.Interfaces;

namespace UltraPlatform.Worker.Services;

internal class DataValidationHelper : IDataValidationHelper
{
    public async Task<bool> SecondDataPrivateValidation()
    {
        return true;
    }

    public async Task<bool> SecondValidation()
    {
        return true;
    }
}