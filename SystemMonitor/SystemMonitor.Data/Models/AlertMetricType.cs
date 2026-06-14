using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.Data.Models;

public enum AlertMetricType
{
    CpuUsage,
    MemoryUsage,
    NetworkReceived,
    NetworkSent,
    DiskRead,
    DiskWrite
}