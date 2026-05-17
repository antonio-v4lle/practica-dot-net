using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Services;

namespace UltraPlatform.Worker.Factories;

public class DataServiceFactory : IDataServiceFactory
{
    private readonly IConfiguration _configuration;

    public DataServiceFactory(
        IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IDataService CreateDataService(IServiceProvider scopedProvider)
    {
        var useFiltering = _configuration
            .GetValue<bool>("Features:UseDataFiltering", defaultValue: true);

        // scopedProvider viene del scope creado en PrimaryBackground
        // NO del root container — scoped safe
        return useFiltering
            ? scopedProvider.GetRequiredService<DataFilteredService>()
            : scopedProvider.GetRequiredService<DataService>();
    }
}
