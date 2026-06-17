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

                Processes.Clear();
                foreach (var process in e.Processes)
                {
                    Processes.Add(process);
                }
            });
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