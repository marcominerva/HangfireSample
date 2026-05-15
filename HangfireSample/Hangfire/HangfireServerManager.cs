using Hangfire;

namespace HangfireSample.Hangfire;

public sealed class HangfireServerManager(JobStorage jobStorage, BackgroundJobServerOptions backgroundJobServerOptions, ILogger<HangfireServerManager> logger)
{
    private readonly Lock syncLock = new();
    private BackgroundJobServer? server;

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

    public void Start()
    {
        lock (syncLock)
        {
            if (server is not null)
            {
                return;
            }

            server = new BackgroundJobServer(backgroundJobServerOptions, jobStorage);
        }

        logger.LogInformation("Hangfire background job server started.");
    }

    public void Stop()
    {
        BackgroundJobServer? serverToDispose;

        lock (syncLock)
        {
            serverToDispose = server;
            server = null;
        }

        if (serverToDispose is null)
        {
            return;
        }

        logger.LogInformation("Stopping Hangfire background job server.");

        serverToDispose.Dispose();

        logger.LogInformation("Hangfire background job server stopped.");
    }
}
