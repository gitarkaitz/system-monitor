using SystemMonitor.Data.Models;

namespace SystemMonitor.Data.Interfaces
{
    public interface IMetricSnapshotRepository
    {
        Task AddAsync(MetricSnapshot snapshot);
        Task<IEnumerable<MetricSnapshot>> GetLatestAsync(int count);
        Task<IEnumerable<MetricSnapshot>> GetBetweenDatesAsync(DateTime from, DateTime to);
        Task DeleteOlderThanAsync(DateTime cutoff);
    }
}
