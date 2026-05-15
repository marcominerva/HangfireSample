using Hangfire.States;

namespace HangfireSample.Hangfire;

internal sealed class CpuAwareHangfireFilter(CpuUsageMonitor cpuUsageMonitor, ILogger<CpuAwareHangfireFilter> logger) : IElectStateFilter
{
    private const double MaxCpuUsagePercentage = 80D;
    private static readonly TimeSpan retryDelay = TimeSpan.FromSeconds(30);

    public void OnStateElection(ElectStateContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.CandidateState is not ProcessingState)
        {
            return;
        }

        var currentCpuUsagePercentage = cpuUsageMonitor.CurrentUsagePercentage;
        if (currentCpuUsagePercentage <= MaxCpuUsagePercentage)
        {
            return;
        }

        logger.LogInformation("Job {JobId} rescheduled after {RetryDelay} because CPU usage is {CpuUsagePercentage:F2}%.",
            context.BackgroundJob.Id, retryDelay, currentCpuUsagePercentage);

        context.CandidateState = new ScheduledState(retryDelay);
    }
}
