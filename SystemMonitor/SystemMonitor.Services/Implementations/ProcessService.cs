using Microsoft.Extensions.Logging;
using System.Diagnostics;
using SystemMonitor.Data.Models;
using SystemMonitor.Services.Interfaces;

namespace SystemMonitor.Services.Implementations
{
    public class ProcessService : IProcessService
    {
        #region Variables

        private readonly ILogger<ProcessService> _logger;

        #endregion

        #region Constructor

        public ProcessService(ILogger<ProcessService> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public Task<IEnumerable<ProcessSnapshot>> GetCurrentProcessesAsync()
        {
            var snapshots = Process.GetProcesses().Select(p => CreateSnapshot(p)).ToList();
            return Task.FromResult<IEnumerable<ProcessSnapshot>>(snapshots);
        }

        #endregion

        #region Private Methods

        private ProcessSnapshot CreateSnapshot(Process process)
        {
            try
            {
                return new ProcessSnapshot
                {
                    ProcessId = process.Id,
                    Name = process.ProcessName,
                    MemoryBytes = process.WorkingSet64,
                    CapturedAt = DateTime.UtcNow,
                    Status = GetProcessStatus(process)
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Access denied reading process {ProcessId}", process.Id);
                return new ProcessSnapshot
                {
                    ProcessId = process.Id,
                    Name = process.ProcessName,
                    CapturedAt = DateTime.UtcNow,
                    Status = ProcessStatus.Unresponsive
                };
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Process {ProcessId} exited before reading", process.Id);
                return new ProcessSnapshot
                {
                    ProcessId = process.Id,
                    Name = process.ProcessName,
                    CapturedAt = DateTime.UtcNow,
                    Status = ProcessStatus.Unresponsive
                };
            }
        }

        private ProcessStatus GetProcessStatus(Process process)
        {
            if (!process.Responding)
            {
                return ProcessStatus.Unresponsive;
            }
            return ProcessStatus.Running;
        }

        #endregion
    }
}