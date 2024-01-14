namespace SleepRemote;

public sealed class WindowsBackgroundService(SleepService svc,
    ILogger<WindowsBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SleepService.StartServer(logger, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // The service is stopping normally. Don't do anything.
        }
        catch (Exception ex)
        {
            // Oh no.
            logger.LogError(ex, "An uncaught exception has been thrown: {Message}", ex.Message);
            Environment.Exit(1);
        }
    }
}