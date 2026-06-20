using Microsoft.Extensions.DependencyInjection;
using SystemMonitor.Data.Interfaces;
using SystemMonitor.Data.Models;
using SystemMonitor.Services.Interfaces;
using System.Windows;

namespace SystemMonitor.UI.Services
{
    public class AlertNotificationService : IDisposable
    {
        #region Fields

        private readonly IAlertService _alertService;
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructor

        public AlertNotificationService(IAlertService alertService, IServiceProvider serviceProvider)
        {
            _alertService = alertService;
            _serviceProvider = serviceProvider;
            _alertService.OnAlertTriggered += AlertService_OnAlertTriggered;
        }

        #endregion

        #region Disposable Implementation

        public void Dispose()
        {
            _alertService.OnAlertTriggered -= AlertService_OnAlertTriggered;
        }

        #endregion

        #region Event Handlers

        private void AlertService_OnAlertTriggered(object? sender, AlertEvent alertEvent)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();
                var alerts = await repository.GetAllActiveAsync();
                var alert = alerts.FirstOrDefault(a => a.Id == alertEvent.AlertId);

                var alertName = alert?.Name ?? "Alerta desconocida";
                var message = $"{alertName}\nValor: {alertEvent.TriggeredValue:F2}";

                var window = new AlertNotificationWindow(message);
                window.ShowDialog();

                if (window.WasAcknowledged)
                {
                    alertEvent.IsAcknowledged = true;
                    await repository.UpdateEventAsync(alertEvent);
                }
            });
        }

        #endregion
    }
}