using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.Data.Models;

public class AlertEvent
{
    #region Attributes

    public int Id { get; set; }
    public int AlertId { get; set; }
    public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;

    public Alert Alert { get; set; } = null!;
    public double TriggeredValue { get; set; }
    public bool IsAcknowledged { get; set; }

    #endregion
}
