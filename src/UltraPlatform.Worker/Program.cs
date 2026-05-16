using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UltraPlatform.Worker.Services;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<DataService>();
        services.AddHostedService<PrimaryBackground>();
    })
    .Build()
    .Run();
