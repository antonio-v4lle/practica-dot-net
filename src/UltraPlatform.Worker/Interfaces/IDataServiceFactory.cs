namespace UltraPlatform.Worker.Interfaces;

/// <summary>
/// Factory for creating appropriate IDataService implementation based on feature flags.
/// </summary>
public interface IDataServiceFactory
{
    IDataService CreateDataService();
}
