using CommunityToolkit.Mvvm.ComponentModel;
using SystemMonitor.Data.Models;
using SystemMonitor.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace SystemMonitor.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        #region Fields

        private readonly IMetricsService _metricsService;
        private readonly IProcessService _processService;
        private readonly DispatcherTimer _timer;

        #endregion

        #region Constructor

        public MainViewModel(IMetricsService metricsService, IProcessService processService)
        {
            _metricsService = metricsService;
            _processService = processService;

            Processes = new ObservableCollection<ProcessSnapshot>();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += async (s, e) => await RefreshDataAsync();
            _timer.Start();
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

        public ObservableCollection<ProcessSnapshot> Processes { get; }

        #endregion

        #region Methods

        private async Task RefreshDataAsync()
        {
            var metrics = await _metricsService.GetCurrentMetricsAsync();

            CpuUsagePercent = metrics.CpuUsagePercent;
            MemoryUsagePercent = metrics.MemoryUsagePercent;
            NetworkBytesReceived = metrics.NetworkBytesReceived;
            NetworkBytesSent = metrics.NetworkBytesSent;
            DiskBytesRead = metrics.DiskBytesRead;
            DiskBytesWritten = metrics.DiskBytesWritten;

            var processes = await _processService.GetCurrentProcessesAsync();

            Processes.Clear();
            foreach (var process in processes)
            {
                Processes.Add(process);
            }
        }

        #endregion
    }
}