using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.Data.Models;

public class Alert
{
    #region Attributes

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsEnabled { get; set; } = true;
    public double Threshold { get; set; }

    public AlertMetricType MetricType { get; set; }
    public AlertCondition Condition { get; set; } = AlertCondition.GreaterThan;
    public ICollection<AlertEvent> Events { get; set; } = [];

    #endregion
}
