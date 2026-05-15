namespace HangfireSample.Hangfire;

internal sealed class HangfireServerHostedService(HangfireServerManager serverManager) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        serverManager.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        serverManager.Stop();

        return Task.CompletedTask;
    }
}
