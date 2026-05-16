using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UltraPlatform.Worker.Services;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Core service
        services.AddSingleton<DataService>();
        
        // Wrapper service (decorator pattern)
        services.AddSingleton<DataFilteredService>();
        
        // Background service
        services.AddHostedService<PrimaryBackground>();
    })
    .Build()
    .Run();
