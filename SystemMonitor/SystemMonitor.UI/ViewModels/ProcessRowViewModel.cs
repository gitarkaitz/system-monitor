using CommunityToolkit.Mvvm.ComponentModel;
using SystemMonitor.Data.Models;

namespace SystemMonitor.UI.ViewModels
{
    public partial class ProcessRowViewModel : ObservableObject
    {
        #region Properties

        public int ProcessId { get; }

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private double _cpuUsagePercent;

        [ObservableProperty]
        private long _memoryBytes;

        [ObservableProperty]
        private ProcessStatus _status;

        #endregion

        #region Constructor

        public ProcessRowViewModel(ProcessSnapshot snapshot)
        {
            ProcessId = snapshot.ProcessId;
            Name = snapshot.Name;
            CpuUsagePercent = snapshot.CpuUsagePercent;
            MemoryBytes = snapshot.MemoryBytes;
            Status = snapshot.Status;
        }

        #endregion

        #region Methods

        public void UpdateFrom(ProcessSnapshot snapshot)
        {
            Name = snapshot.Name;
            CpuUsagePercent = snapshot.CpuUsagePercent;
            MemoryBytes = snapshot.MemoryBytes;
            Status = snapshot.Status;
        }

        #endregion
    }
}