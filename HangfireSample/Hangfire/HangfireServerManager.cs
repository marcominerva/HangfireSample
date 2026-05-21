using Hangfire;
using Hangfire.Server;

namespace HangfireSample.Hangfire;

public class HangfireServerManager(JobStorage jobStorage, BackgroundJobServerOptions backgroundJobServerOptions, IEnumerable<IBackgroundProcess> additionalProcesses, ILogger<HangfireServerManager> logger)
{
    private readonly Lock syncLock = new();
    private IBackgroundProcessingServer? server;

    public bool IsRunning
    {
        get
        {
            lock (syncLock)
            {
                return server is not null;
            }
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (syncLock)
        {
            if (server is not null)
            {
                return Task.CompletedTask;
            }

            server = new BackgroundJobServer(backgroundJobServerOptions, jobStorage, additionalProcesses);
        }

        logger.LogInformation("Hangfire background job server started.");

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        IBackgroundProcessingServer? serverToStop;

        lock (syncLock)
        {
            serverToStop = server;
            server = null;
        }

        if (serverToStop is null)
        {
            return;
        }

        logger.LogInformation("Stopping Hangfire background job server.");

        try
        {
            serverToStop.SendStop();
            await serverToStop.WaitForShutdownAsync(cancellationToken);
        }
        catch (ObjectDisposedException)
        {
        }

        serverToStop.Dispose();

        logger.LogInformation("Hangfire background job server stopped.");
    }
}
