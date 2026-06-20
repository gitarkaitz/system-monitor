using SystemMonitor.Data.Models;

namespace SystemMonitor.Data.Interfaces
{
    public interface IAlertRepository
    {
        Task AddAsync(Alert alert);
        Task UpdateAsync(Alert alert);
        Task<IEnumerable<Alert>> GetAllActiveAsync();
        Task AddEventAsync(AlertEvent alertEvent);
        Task<IEnumerable<AlertEvent>> GetRecentEventsAsync(int count);
        Task UpdateEventAsync(AlertEvent alertEvent);
    }
}
