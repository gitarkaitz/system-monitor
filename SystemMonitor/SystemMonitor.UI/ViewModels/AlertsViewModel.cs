using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SystemMonitor.Data.Interfaces;
using SystemMonitor.Data.Models;
using System.Collections.ObjectModel;

namespace SystemMonitor.UI.ViewModels
{
    public partial class AlertsViewModel : ObservableObject
    {
        #region Fields

        private readonly IAlertRepository _alertRepository;

        #endregion

        #region Properties

        public ObservableCollection<Alert> Alerts { get; } = new();

        public ObservableCollection<AlertEvent> RecentEvents { get; } = new();

        [ObservableProperty]
        private bool _hasRecentEvents;

        #endregion

        #region Constructor

        public AlertsViewModel(IAlertRepository alertRepository)
        {
            _alertRepository = alertRepository;
            _ = LoadAsync();
        }

        #endregion

        #region Methods

        private async Task LoadAsync()
        {
            var alerts = await _alertRepository.GetAllActiveAsync();
            Alerts.Clear();
            foreach (var alert in alerts)
            {
                Alerts.Add(alert);
            }

            var events = await _alertRepository.GetRecentEventsAsync(20);
            RecentEvents.Clear();
            foreach (var ev in events)
            {
                RecentEvents.Add(ev);
            }

            HasRecentEvents = RecentEvents.Any();
        }

        [RelayCommand]
        private async Task ToggleAlertAsync(Alert alert)
        {
            alert.IsEnabled = !alert.IsEnabled;
            await _alertRepository.UpdateAsync(alert);
        }

        #endregion
    }
}