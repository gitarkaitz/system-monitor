using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using SystemMonitor.Data.Models;
using SystemMonitor.Services.Implementations;
using SystemMonitor.Services.Interfaces;

namespace SystemMonitor.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        #region Fields

        private readonly IScheduler _scheduler;

        private List<ProcessSnapshot> _allProcesses = [];

        private readonly IProcessService _processService;

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

        public ObservableCollection<ProcessRowViewModel> Processes { get; }

        [ObservableProperty]
        private ProcessRowViewModel? _selectedProcess;

        #endregion

        #region Constructor

        public MainViewModel(IScheduler scheduler, IProcessService processService) 
        {
            _scheduler = scheduler;
            _processService = processService;
            Processes = new ObservableCollection<ProcessRowViewModel>();
            _scheduler.OnTick += Scheduler_OnTick;
        }

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
                UpdateProcessRows();
            });
        }


        #endregion

        #region Process related

        partial void OnFilterTextChanged(string value)
        {
            ApplyFilter();
        }

        private void UpdateProcessRows()
        {
            var currentIds = _allProcesses.Select(p => p.ProcessId).ToHashSet();

            var toRemove = Processes.Where(row => !currentIds.Contains(row.ProcessId)).ToList();
            foreach (var row in toRemove)
            {
                Processes.Remove(row);
            }

            foreach (var snapshot in _allProcesses)
            {
                var existingRow = Processes.FirstOrDefault(row => row.ProcessId == snapshot.ProcessId);

                if (existingRow != null)
                {
                    existingRow.UpdateFrom(snapshot);
                }
                else
                {
                    Processes.Add(new ProcessRowViewModel(snapshot));
                }
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                return;
            }

            var toHide = Processes.Where(row => !row.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var row in toHide)
            {
                Processes.Remove(row);
            }
        }

        [RelayCommand(CanExecute = nameof(CanStopProcess))]
        private void StopProcess()
        {
            if (SelectedProcess == null)
            {
                return;
            }

            var success = _processService.KillProcess(SelectedProcess.ProcessId);

            if (success)
            {
                _allProcesses.RemoveAll(p => p.ProcessId == SelectedProcess.ProcessId);
                ApplyFilter();
            }
        }

        private bool CanStopProcess()
        {
            return SelectedProcess != null;
        }

        partial void OnSelectedProcessChanged(ProcessRowViewModel? value)
        {
            StopProcessCommand.NotifyCanExecuteChanged();
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