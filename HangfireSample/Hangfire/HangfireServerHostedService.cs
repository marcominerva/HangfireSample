namespace HangfireSample.Hangfire;

internal sealed class HangfireServerHostedService(HangfireServerManager serverManager) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return serverManager.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return serverManager.StopAsync(cancellationToken);
    }
}
