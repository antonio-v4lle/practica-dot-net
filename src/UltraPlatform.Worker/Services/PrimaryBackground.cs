using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UltraPlatform.Worker.Services;

internal class PrimaryBackground : BackgroundService
{
    private readonly ILogger<PrimaryBackground> _logger;

    public PrimaryBackground(ILogger<PrimaryBackground> logger)
    {
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        _logger.LogInformation("Background service started.");
        while (!stoppingToken.IsCancellationRequested)
        {
            // Your console logic here
            _logger.LogInformation("Running task at: {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        }

        // Console.WriteLine("Hola mundo");
        // return Task.CompletedTask;
    }
}