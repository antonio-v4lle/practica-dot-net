using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UltraPlatform.Worker.Interfaces;

namespace UltraPlatform.Worker.Services;

internal class PrimaryBackground : BackgroundService
{
    private readonly ILogger<PrimaryBackground> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public PrimaryBackground(
        ILogger<PrimaryBackground> logger, 
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
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

                var svc = factory.CreateDataService(scope.ServiceProvider);

                var isValid = await svc.ComposeValidation();

                _logger.LogInformation("Validation result: {isValid} at {time}, Service: {service}", isValid, DateTimeOffset.Now, svc.GetType().Name);
                
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Background service cancelled.");
                break;
            }
        }
    }
}
