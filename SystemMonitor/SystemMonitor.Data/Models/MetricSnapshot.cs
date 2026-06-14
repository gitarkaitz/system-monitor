using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.Data.Models;

public class MetricSnapshot
{
    #region Attributes

    public int Id { get; set; }
    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;

    public double CpuUsagePercent { get; set; }
    public long MemoryUsedBytes { get; set; }
    public long MemoryTotalBytes { get; set; }

    public long NetworkBytesReceived { get; set; }
    public long NetworkBytesSent { get; set; }

    public long DiskBytesRead { get; set; }
    public long DiskBytesWritten { get; set; }

    #endregion

    #region Computed Properties

    public double MemoryUsagePercent => MemoryTotalBytes > 0 ? (double)MemoryUsedBytes / MemoryTotalBytes * 100.0 : 0;

    #endregion
}
