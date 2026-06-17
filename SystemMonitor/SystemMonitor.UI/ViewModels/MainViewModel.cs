using CommunityToolkit.Mvvm.ComponentModel;
using SystemMonitor.Data.Models;
using SystemMonitor.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;

namespace SystemMonitor.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        #region Fields

        private readonly IScheduler _scheduler;

        private List<ProcessSnapshot> _allProcesses = [];

        #endregion

        #region Constructor

        public MainViewModel(IScheduler scheduler)
        {
            _scheduler = scheduler;
            Processes = new ObservableCollection<ProcessSnapshot>();
            _scheduler.OnTick += Scheduler_OnTick;
        }

        #endregion

        #region Properties

        [ObservableProperty]
        private double _cpuUsagePercent;

        [ObservableProperty]
        private double _memoryUsagePercent;

        [ObservableProperty]
        private long _networkBytesReceived;

        [ObservableProperty]
        private long _networkBytesSent;

        [ObservableProperty]
        private long _diskBytesRead;

        [ObservableProperty]
        private long _diskBytesWritten;

        [ObservableProperty]
        private string _filterText = string.Empty;

        public ObservableCollection<ProcessSnapshot> Processes { get; }

        #endregion

        #region Event Handlers

        private void Scheduler_OnTick(object? sender, SchedulerTickEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CpuUsagePercent = e.Metrics.CpuUsagePercent;
                MemoryUsagePercent = e.Metrics.MemoryUsagePercent;
                NetworkBytesReceived = e.Metrics.NetworkBytesReceived;
                NetworkBytesSent = e.Metrics.NetworkBytesSent;
                DiskBytesRead = e.Metrics.DiskBytesRead;
                DiskBytesWritten = e.Metrics.DiskBytesWritten;

                _allProcesses = e.Processes.ToList();
                ApplyFilter();
            });
        }

        #endregion

        #region Process related

        partial void OnFilterTextChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = string.IsNullOrWhiteSpace(FilterText)
                ? _allProcesses
                : _allProcesses.Where(p => p.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();

            Processes.Clear();
            foreach (var process in filtered)
            {
                Processes.Add(process);
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            _scheduler.OnTick -= Scheduler_OnTick;
        }

        #endregion
    }
}