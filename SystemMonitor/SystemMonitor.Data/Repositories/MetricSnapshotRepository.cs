using Microsoft.EntityFrameworkCore;
using SystemMonitor.Data.Interfaces;
using SystemMonitor.Data.Models;

namespace SystemMonitor.Data.Repositories
{
    public class MetricSnapshotRepository : IMetricSnapshotRepository
    {
        #region Fields

        private readonly AppDbContext _context;

        #endregion

        #region Constructor

        public MetricSnapshotRepository(AppDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        public async Task AddAsync(MetricSnapshot snapshot)
        {
            await _context.MetricSnapshots.AddAsync(snapshot);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<MetricSnapshot> snapshots)
        {
            await _context.MetricSnapshots.AddRangeAsync(snapshots);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MetricSnapshot>> GetLatestAsync(int count)
        {
            return await _context.MetricSnapshots.OrderByDescending(s => s.CapturedAt).Take(count).ToListAsync();
        }

        public async Task<IEnumerable<MetricSnapshot>> GetBetweenDatesAsync(DateTime from, DateTime to)
        {
            return await _context.MetricSnapshots.Where(s => s.CapturedAt >= from && s.CapturedAt <= to).OrderBy(s => s.CapturedAt).ToListAsync();
        }

        public async Task DeleteOlderThanAsync(DateTime cutoff)
        {
            await _context.MetricSnapshots.Where(s => s.CapturedAt < cutoff).ExecuteDeleteAsync();
        }
        #endregion
    }
}
