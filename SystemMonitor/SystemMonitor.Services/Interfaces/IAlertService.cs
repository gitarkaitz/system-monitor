using SystemMonitor.Data.Models;

namespace SystemMonitor.Services.Interfaces
{
    public interface IAlertService
    {
        Task EvaluateAsync(MetricSnapshot metrics);
        Task<IEnumerable<Alert>> GetActiveAlertsAsync();
        Task AddAlertAsync(Alert alert);
        Task UpdateAlertAsync(Alert alert);
        event EventHandler<AlertEvent>? OnAlertTriggered;
    }
}