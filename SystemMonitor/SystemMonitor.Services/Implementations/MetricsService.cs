using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.Versioning;
using SystemMonitor.Data.Models;
using SystemMonitor.Services.Interfaces;

namespace SystemMonitor.Services.Implementations
{
    [SupportedOSPlatform("windows")]
    public class MetricsService : IMetricsService
    {
        #region Fields

        private readonly ILogger<MetricsService> _logger;
        private PerformanceCounter? _cpuCounter;
        private PerformanceCounter? _ramCounter;
        private PerformanceCounter? _networkReceivedCounter;
        private PerformanceCounter? _networkSentCounter;
        private PerformanceCounter? _diskReadCounter;
        private PerformanceCounter? _diskWriteCounter;

        #endregion

        #region Constructor

        public MetricsService(ILogger<MetricsService> logger)
        {
            _logger = logger;
            InitializeCounters();
        }

        #endregion

        #region Public Methods

        public async Task<MetricSnapshot> GetCurrentMetricsAsync()
        {
            try
            {
                await Task.Delay(500);

                return new MetricSnapshot
                {
                    CapturedAt = DateTime.UtcNow,
                    CpuUsagePercent = Math.Round(_cpuCounter?.NextValue() ?? 0, 2),
                    MemoryUsedBytes = GetUsedMemoryBytes(),
                    MemoryTotalBytes = GetTotalMemoryBytes(),
                    NetworkBytesReceived = (long)(_networkReceivedCounter?.NextValue() ?? 0),
                    NetworkBytesSent = (long)(_networkSentCounter?.NextValue() ?? 0),
                    DiskBytesRead = (long)(_diskReadCounter?.NextValue() ?? 0),
                    DiskBytesWritten = (long)(_diskWriteCounter?.NextValue() ?? 0)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading system metrics");
                return new MetricSnapshot { CapturedAt = DateTime.UtcNow };
            }
        }

        #endregion

        #region Private Methods

        private void InitializeCounters()
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                _networkReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", GetNetworkInterface());
                _networkSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", GetNetworkInterface());
                _diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
                _diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");

                _cpuCounter.NextValue();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing performance counters");
            }
        }

        private string GetNetworkInterface()
        {
            var category = new PerformanceCounterCategory("Network Interface");
            var interfaces = category.GetInstanceNames();
            return interfaces.FirstOrDefault() ?? string.Empty;
        }

        private long GetUsedMemoryBytes()
        {
            var availableMB = (_ramCounter?.NextValue() ?? 0);
            var totalMB = GetTotalMemoryBytes() / (1024 * 1024);
            return (long)((totalMB - availableMB) * 1024 * 1024);
        }

        private long GetTotalMemoryBytes()
        {
            var searcher = new System.Management.ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");

            foreach (var obj in searcher.Get())
            {
                return (long)(ulong)obj["TotalPhysicalMemory"];
            }
            return 0;
        }

        #endregion
    }
}