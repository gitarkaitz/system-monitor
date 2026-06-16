using SystemMonitor.Data.Models;

namespace SystemMonitor.Services.Interfaces
{
    public interface IProcessService
    {
        Task<IEnumerable<ProcessSnapshot>> GetCurrentProcessesAsync();
    }
}