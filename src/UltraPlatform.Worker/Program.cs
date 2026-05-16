using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Services;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Core service (not directly injected)
        services.AddSingleton<DataService>();
        
        // Wrapper service implements IDataService (primary implementation)
        services.AddSingleton<IDataService, DataFilteredService>();
        
        // Background service
        services.AddHostedService<PrimaryBackground>();
    })
    .Build()
    .Run();
