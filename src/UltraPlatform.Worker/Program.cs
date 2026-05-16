using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UltraPlatform.Worker.Factories;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Services;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Core service
        services.AddSingleton<DataService>();
        
        // Wrapper service
        services.AddSingleton<DataFilteredService>();
        
        // Factory to create appropriate IDataService based on feature flags
        services.AddSingleton<IDataServiceFactory, DataServiceFactory>();
        
        // Background service - gets IDataService from factory
        services.AddHostedService<PrimaryBackground>();
    })
    .Build()
    .Run();
