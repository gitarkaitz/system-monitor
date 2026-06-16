using Microsoft.Extensions.Logging;
using SystemMonitor.Data.Interfaces;
using SystemMonitor.Data.Models;
using SystemMonitor.Services.Interfaces;

namespace SystemMonitor.Services.Implementations
{
    public class AlertService : IAlertService
    {
        #region Fields

        private readonly IAlertRepository _alertRepository;
        private readonly ILogger<AlertService> _logger;

        #endregion

        #region Constructor

        public AlertService(IAlertRepository alertRepository, ILogger<AlertService> logger)
        {
            _alertRepository = alertRepository;
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public async Task EvaluateAsync(MetricSnapshot metrics)
        {
            var activeAlerts = await _alertRepository.GetAllActiveAsync();

            foreach (var alert in activeAlerts)
            {
                var currentValue = GetMetricValue(metrics, alert.MetricType);
                if (IsThresholdExceeded(currentValue, alert.Threshold, alert.Condition))
                {
                    await TriggerAlertAsync(alert, currentValue);
                }
            }
        }

        public async Task<IEnumerable<Alert>> GetActiveAlertsAsync()
        {
            return await _alertRepository.GetAllActiveAsync();
        }

        public async Task AddAlertAsync(Alert alert)
        {
            await _alertRepository.AddAsync(alert);
            _logger.LogInformation("Alert '{AlertName}' added", alert.Name);
        }

        public async Task UpdateAlertAsync(Alert alert)
        {
            await _alertRepository.UpdateAsync(alert);
            _logger.LogInformation("Alert '{AlertName}' updated", alert.Name);
        }

        #endregion

        #region Private Methods

        private double GetMetricValue(MetricSnapshot metrics, AlertMetricType metricType)
        {
            switch (metricType)
            {
                case AlertMetricType.CpuUsage:
                    return metrics.CpuUsagePercent;

                case AlertMetricType.MemoryUsage:
                    return metrics.MemoryUsagePercent;

                case AlertMetricType.NetworkReceived:
                    return metrics.NetworkBytesReceived;

                case AlertMetricType.NetworkSent:
                    return metrics.NetworkBytesSent;

                case AlertMetricType.DiskRead:
                    return metrics.DiskBytesRead;

                case AlertMetricType.DiskWrite:
                    return metrics.DiskBytesWritten;

                default:
                    return 0;
            }
        }

        private bool IsThresholdExceeded(double currentValue, double threshold, AlertCondition condition)
        {
            switch (condition)
            {
                case AlertCondition.GreaterThan:
                    return currentValue > threshold;

                case AlertCondition.LessThan:
                    return currentValue < threshold;

                default:
                    return false;
            }
        }

        private async Task TriggerAlertAsync(Alert alert, double triggeredValue)
        {
            var alertEvent = new AlertEvent
            {
                AlertId = alert.Id,
                TriggeredValue = triggeredValue,
                TriggeredAt = DateTime.UtcNow,
                IsAcknowledged = false
            };

            await _alertRepository.AddEventAsync(alertEvent);

            _logger.LogWarning("Alert '{AlertName}' triggered. Value: {Value}, Threshold: {Threshold}", alert.Name, triggeredValue, alert.Threshold);
        }

        #endregion
    }
}