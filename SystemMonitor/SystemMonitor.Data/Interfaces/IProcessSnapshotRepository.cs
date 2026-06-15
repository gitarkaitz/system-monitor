using SystemMonitor.Data.Models;

namespace SystemMonitor.Data.Interfaces
{
    public interface IProcessSnapshotRepository
    {
        Task AddRangeAsync(IEnumerable<ProcessSnapshot> snapshots);
        Task<IEnumerable<ProcessSnapshot>> GetLatestAsync();
        Task<IEnumerable<ProcessSnapshot>> GetByNameAsync(string name);
        Task DeleteOlderThanAsync(DateTime cutoff);
    }
}
