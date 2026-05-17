using Microsoft.Extensions.Configuration;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Services;

namespace UltraPlatform.Worker.Factories;

public class DataServiceFactory : IDataServiceFactory
{
    private readonly IConfiguration _configuration;
    private readonly DataService _coreDataService;
    private readonly DataFilteredService _filteredDataService;

    public DataServiceFactory(
        IConfiguration configuration,
        DataService coreDataService,
        DataFilteredService filteredDataService)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _coreDataService = coreDataService ?? throw new ArgumentNullException(nameof(coreDataService));
        _filteredDataService = filteredDataService ?? throw new ArgumentNullException(nameof(filteredDataService));
    }

    public IDataService CreateDataService()
    {
        // Check feature flag from configuration
        var useFiltering = _configuration.GetValue<bool>("Features:UseDataFiltering", defaultValue: true);

        if (useFiltering)
        {
            return _filteredDataService;
        }

        return _coreDataService;
    }
}
