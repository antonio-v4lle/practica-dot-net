
namespace UltraPlatform.Worker.Interfaces;

public interface IDataService
{
    Task<bool> ComposeValidation();
    Task<bool> PublicContractMethod();
}