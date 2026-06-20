using Microsoft.EntityFrameworkCore;
using SystemMonitor.Data.Interfaces;
using SystemMonitor.Data.Models;

namespace SystemMonitor.Data.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        #region Fields

        private readonly AppDbContext _context;

        #endregion

        #region Constructor

        public AlertRepository(AppDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        public async Task AddAsync(Alert alert)
        {
            await _context.Alerts.AddAsync(alert);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Alert> alerts)
        {
            await _context.Alerts.AddRangeAsync(alerts);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Alert alert)
        {
            _context.Alerts.Update(alert);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Alert>> GetAllActiveAsync()
        {
            return await _context.Alerts.Where(a => a.IsEnabled).ToListAsync();
        }

        public async Task AddEventAsync(AlertEvent alertEvent)
        {
            await _context.AlertEvents.AddAsync(alertEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AlertEvent>> GetRecentEventsAsync(int count)
        {
            return await _context.AlertEvents
                .OrderByDescending(e => e.TriggeredAt)
                .Take(count)
                .Include(e => e.Alert)
                .ToListAsync();
        }

        public async Task DeleteOlderThanAsync(DateTime cutoff)
        {
            await _context.AlertEvents.Where(e => e.TriggeredAt < cutoff).ExecuteDeleteAsync();
        }

        public async Task UpdateEventAsync(AlertEvent alertEvent)
        {
            _context.AlertEvents.Update(alertEvent);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}