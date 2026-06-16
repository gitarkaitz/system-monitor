using SystemMonitor.Data.Models;

namespace SystemMonitor.Services.Interfaces
{
    public interface IMetricsService
    {
        Task<MetricSnapshot> GetCurrentMetricsAsync();
    }
}