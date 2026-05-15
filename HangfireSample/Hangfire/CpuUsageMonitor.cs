using System.Runtime.InteropServices;

namespace HangfireSample.Hangfire;

internal sealed class CpuUsageMonitor
{
    private readonly Lock syncLock = new();
    private readonly bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    private CpuTimes? lastCpuTimes;
    private double lastUsagePercentage;

    public double CurrentUsagePercentage
    {
        get
        {
            if (!isWindows)
            {
                return 0D;
            }

            lock (syncLock)
            {
                if (!TryReadCpuTimes(out var currentCpuTimes))
                {
                    return lastUsagePercentage;
                }

                if (lastCpuTimes is null)
                {
                    lastCpuTimes = currentCpuTimes;

                    return lastUsagePercentage;
                }

                var totalDelta = currentCpuTimes.Total - lastCpuTimes.Value.Total;
                if (totalDelta <= 0)
                {
                    lastCpuTimes = currentCpuTimes;

                    return lastUsagePercentage;
                }

                var idleDelta = currentCpuTimes.Idle - lastCpuTimes.Value.Idle;
                var busyDelta = Math.Max(0L, totalDelta - idleDelta);
                lastUsagePercentage = Math.Clamp((double)busyDelta / totalDelta * 100D, 0D, 100D);
                lastCpuTimes = currentCpuTimes;

                return lastUsagePercentage;
            }
        }
    }

    private static bool TryReadCpuTimes(out CpuTimes cpuTimes)
    {
        if (!GetSystemTimes(out var idleTime, out var kernelTime, out var userTime))
        {
            cpuTimes = default;

            return false;
        }

        var idle = ToInt64(idleTime);
        var kernel = ToInt64(kernelTime);
        var user = ToInt64(userTime);
        cpuTimes = new CpuTimes(idle, kernel + user);

        return true;
    }

    private static long ToInt64(FILETIME fileTime) => ((long)fileTime.dwHighDateTime << 32) | fileTime.dwLowDateTime;

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetSystemTimes(out FILETIME idleTime, out FILETIME kernelTime, out FILETIME userTime);

    private readonly record struct CpuTimes(long Idle, long Total);

    [StructLayout(LayoutKind.Sequential)]
    private struct FILETIME
    {
        public uint dwLowDateTime;

        public uint dwHighDateTime;
    }
}
