using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UltraPlatform.Worker.Interfaces;

namespace UltraPlatform.Worker.Services;

internal class PrimaryBackground : BackgroundService
{
    private readonly ILogger<PrimaryBackground> _logger;
    private readonly IDataService _dataService;
    private readonly IServiceScopeFactory _scopeFactory;

    public PrimaryBackground(
        ILogger<PrimaryBackground> logger, 
        IServiceScopeFactory scopeFactory,
        IDataServiceFactory dataServiceFactory)
    {
        _logger = logger;
        _dataService = dataServiceFactory.CreateDataService();
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background service started.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var factory = scope.ServiceProvider.GetRequiredService<IDataServiceFactory>();

                var svc = factory.CreateDataService();

                var isValid = await svc.ComposeValidation();

                _logger.LogInformation("Validation result: {isValid} at {time}", isValid, DateTimeOffset.Now);
                
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Background service cancelled.");
                break;
            }
        }
    }
}
