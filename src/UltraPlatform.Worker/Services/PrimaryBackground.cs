using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UltraPlatform.Worker.Interfaces;

namespace UltraPlatform.Worker.Services;

internal class PrimaryBackground : BackgroundService
{
    private readonly ILogger<PrimaryBackground> _logger;
    private readonly IDataService _dataService;

    public PrimaryBackground(ILogger<PrimaryBackground> logger, IDataServiceFactory dataServiceFactory)
    {
        _logger = logger;
        _dataService = dataServiceFactory.CreateDataService();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background service started.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var isValid = await _dataService.ComposeValidation();
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
