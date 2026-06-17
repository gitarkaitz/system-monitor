using SystemMonitor.Data.Models;

namespace SystemMonitor.Services.Interfaces
{
    public class SchedulerTickEventArgs : EventArgs
    {
        public MetricSnapshot Metrics { get; init; } = null!;
        public IEnumerable<ProcessSnapshot> Processes { get; init; } = [];
    }
}