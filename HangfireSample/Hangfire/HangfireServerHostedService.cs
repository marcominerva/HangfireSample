namespace HangfireSample.Hangfire;

public class HangfireServerHostedService(HangfireServerManager serverManager) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
        => serverManager.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken)
        => serverManager.StopAsync(cancellationToken);
}
