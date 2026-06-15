using Microsoft.EntityFrameworkCore;
using SystemMonitor.Data;
using SystemMonitor.Data.Models;
using SystemMonitor.Data.Repositories;

namespace SystemMonitor.Tests.Repositories
{
    public class MetricSnapshotRepositoryTests
    {
        #region Setup

        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite("DataSource=:memory:").Options;
            var context = new AppDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            return context;
        }

        #endregion

        #region AddAsync

        [Fact]
        public async Task AddAsync_ShouldPersistSnapshot()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);
            var snapshot = new MetricSnapshot
            {
                CpuUsagePercent = 45.5,
                MemoryUsedBytes = 4000000000,
                MemoryTotalBytes = 8000000000
            };

            await repository.AddAsync(snapshot);
            var result = await context.MetricSnapshots.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(45.5, result.CpuUsagePercent);
        }

        #endregion

        #region GetLatestAsync

        [Fact]
        public async Task GetLatestAsync_ShouldReturnRequestedCount()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);

            for (int i = 0; i < 10; i++)
            {
                await repository.AddAsync(new MetricSnapshot
                {
                    CapturedAt = DateTime.UtcNow.AddMinutes(-i),
                    CpuUsagePercent = i * 10
                });
            }
            var result = await repository.GetLatestAsync(5);
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public async Task GetLatestAsync_ShouldReturnMostRecentFirst()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);
            var oldest = DateTime.UtcNow.AddMinutes(-10);
            var newest = DateTime.UtcNow;

            await repository.AddAsync(new MetricSnapshot { CapturedAt = oldest });
            await repository.AddAsync(new MetricSnapshot { CapturedAt = newest });

            var result = (await repository.GetLatestAsync(2)).ToList();
            Assert.True(result[0].CapturedAt > result[1].CapturedAt);
        }
         
        #endregion

        #region GetBetweenDatesAsync

        [Fact]
        public async Task GetBetweenDatesAsync_ShouldReturnOnlySnapshotsInRange()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);
            var now = DateTime.UtcNow;

            await repository.AddAsync(new MetricSnapshot { CapturedAt = now.AddHours(-3) });
            await repository.AddAsync(new MetricSnapshot { CapturedAt = now.AddHours(-1) });
            await repository.AddAsync(new MetricSnapshot { CapturedAt = now.AddHours(1) });

            var result = await repository.GetBetweenDatesAsync(now.AddHours(-2), now);
            Assert.Single(result);

        }

        #endregion

        #region DeleteOlderThanAsync

        [Fact]
        public async Task DeleteOlderThanAsync_ShouldDeleteOnlyOldSnapshots()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);
            var now = DateTime.UtcNow;

            await repository.AddAsync(new MetricSnapshot { CapturedAt = now.AddDays(-2) });
            await repository.AddAsync(new MetricSnapshot { CapturedAt = now.AddDays(-2) });
            await repository.AddAsync(new MetricSnapshot { CapturedAt = now });
            await repository.DeleteOlderThanAsync(now.AddDays(-1));

            var remaining = await context.MetricSnapshots.CountAsync();
            Assert.Equal(1, remaining);
        }

        #endregion

        #region Edge Cases - AddAsync

        [Fact]
        public async Task AddAsync_WithZeroValues_ShouldPersistCorrectly()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);
            var snapshot = new MetricSnapshot
            {
                CpuUsagePercent = 0,
                MemoryUsedBytes = 0,
                MemoryTotalBytes = 0
            };

            await repository.AddAsync(snapshot);
            var result = await context.MetricSnapshots.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(0, result.CpuUsagePercent);
            Assert.Equal(0, result.MemoryUsedBytes);
        }
        #endregion

        #region Edge Cases - GetLatestAsync
        [Fact]
        public async Task GetLatestAsync_WithCountGreaterThanRecords_ShouldReturnAllRecords()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);

            for (int i = 0; i < 3; i++)
            {
                await repository.AddAsync(new MetricSnapshot
                {
                    CapturedAt = DateTime.UtcNow.AddMinutes(-i)
                });
            }
            var result = await repository.GetLatestAsync(10);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetLatestAsync_WithEmptyDatabase_ShouldReturnEmptyCollection()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);
            var result = await repository.GetLatestAsync(5);
            Assert.Empty(result);
        }
        #endregion

        #region Edge Cases - GetBetweenDatesAsync
        [Fact]
        public async Task GetBetweenDatesAsync_WithNoMatchingRecords_ShouldReturnEmptyCollection()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);
            var now = DateTime.UtcNow;

            await repository.AddAsync(new MetricSnapshot { CapturedAt = now.AddDays(-5) });
            await repository.AddAsync(new MetricSnapshot { CapturedAt = now.AddDays(-4) });
            var result = await repository.GetBetweenDatesAsync(now.AddDays(-2), now);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetBetweenDatesAsync_WithInvertedRange_ShouldReturnEmptyCollection()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);
            var now = DateTime.UtcNow;

            await repository.AddAsync(new MetricSnapshot { CapturedAt = now });
            var result = await repository.GetBetweenDatesAsync(now.AddHours(1), now.AddHours(-1));
            Assert.Empty(result);
        }
        #endregion

        #region Edge Cases - DeleteOlderThanAsync

        [Fact]
        public async Task DeleteOlderThanAsync_WithEmptyDatabase_ShouldNotThrow()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);
            var exception = await Record.ExceptionAsync(() => repository.DeleteOlderThanAsync(DateTime.UtcNow));
            Assert.Null(exception);
        }

        [Fact]
        public async Task DeleteOlderThanAsync_WithNoMatchingRecords_ShouldNotDeleteAnything()
        {
            using var context = CreateContext();
            var repository = new MetricSnapshotRepository(context);
            var now = DateTime.UtcNow;

            await repository.AddAsync(new MetricSnapshot { CapturedAt = now });
            await repository.AddAsync(new MetricSnapshot { CapturedAt = now.AddMinutes(1) });
            await repository.DeleteOlderThanAsync(now.AddDays(-1));
            var remaining = await context.MetricSnapshots.CountAsync();
            Assert.Equal(2, remaining);
        }

        #endregion

    }
}