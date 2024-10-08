using System.Net;
using System.Runtime.InteropServices;

public readonly struct EnvironmentInfo
{
    public EnvironmentInfo()
    {
        GCMemoryInfo gcInfo = GC.GetGCMemoryInfo();
        TotalAvailableMemoryBytes = gcInfo.TotalAvailableMemoryBytes;

        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        string[] memoryLimitPaths = new string[] 
        {
            "/sys/fs/cgroup/memory.max",
            "/sys/fs/cgroup/memory.high",
            "/sys/fs/cgroup/memory.low",
            "/sys/fs/cgroup/memory/memory.limit_in_bytes",
        };

        string[] currentMemoryPaths = new string[] 
        {
            "/sys/fs/cgroup/memory.current",
            "/sys/fs/cgroup/memory/memory.usage_in_bytes",
        };

        MemoryLimit = GetBestValue(memoryLimitPaths);
        MemoryUsage = GetBestValue(currentMemoryPaths);

        try
        {
            FipsEnabled = File.ReadAllText("/proc/sys/crypto/fips_enabled").Trim();
        }
        catch (Exception)
        {
        }
    }

    public string RuntimeVersion => RuntimeInformation.FrameworkDescription;
    public string OSVersion => RuntimeInformation.OSDescription;
    public string OSArchitecture => RuntimeInformation.OSArchitecture.ToString();
    public string User => Environment.UserName;
    public int ProcessorCount => Environment.ProcessorCount;
    public long TotalAvailableMemoryBytes { get; }
    public long? MemoryLimit { get; }
    public long? MemoryUsage { get; }
    public string HostName => Dns.GetHostName();
    public string? FipsEnabled { get; }

    private static long? GetBestValue(string[] paths)
    {
        foreach (string path in paths)
        {
            if (Path.Exists(path) &&
                long.TryParse(File.ReadAllText(path), out long result))
            {
                return result;
            }
        }

        return null;
    }
}