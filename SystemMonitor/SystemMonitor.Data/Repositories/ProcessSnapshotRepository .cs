using Microsoft.EntityFrameworkCore;
using SystemMonitor.Data.Interfaces;
using SystemMonitor.Data.Models;

namespace SystemMonitor.Data.Repositories
{
    public class ProcessSnapshotRepository : IProcessSnapshotRepository
    {
        #region Variabeles

        private readonly AppDbContext _context;

        #endregion

        #region Constructor

        public ProcessSnapshotRepository(AppDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        public async Task AddRangeAsync(IEnumerable<ProcessSnapshot> snapshots)
        {
            await _context.ProcessSnapshots.AddRangeAsync(snapshots);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProcessSnapshot>> GetLatestAsync()
        {
            return await _context.ProcessSnapshots
                .GroupBy(s => s.ProcessId)
                .Select(g => g.OrderByDescending(s => s.CapturedAt).First())
                .ToListAsync();
        }

        public async Task<IEnumerable<ProcessSnapshot>> GetByNameAsync(string name)
        {
            return await _context.ProcessSnapshots
                .Where(s => s.Name.Contains(name))
                .OrderByDescending(s => s.CapturedAt)
                .ToListAsync();
        }

        public async Task DeleteOlderThanAsync(DateTime cutoff)
        {
            await _context.ProcessSnapshots
                .Where(s => s.CapturedAt < cutoff)
                .ExecuteDeleteAsync();
        }

        #endregion
    }
}