using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.Data.Models;

public class ProcessSnapshot
{
    #region Attributes

    public int Id { get; set; }
    public int ProcessId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;

    public double CpuUsagePercent { get; set; }
    public long MemoryBytes { get; set; }

    public ProcessStatus Status { get; set; } = ProcessStatus.Running;

    #endregion
}
