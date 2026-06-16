using Moq;
using SystemMonitor.Data.Interfaces;
using SystemMonitor.Data.Models;
using SystemMonitor.Services.Implementations;
using Microsoft.Extensions.Logging;

namespace SystemMonitor.Tests.Services
{
    public class AlertServiceTests
    {
        #region Fields

        private readonly Mock<IAlertRepository> _alertRepositoryMock;
        private readonly Mock<ILogger<AlertService>> _loggerMock;
        private readonly AlertService _alertService;

        #endregion

        #region Setup

        public AlertServiceTests()
        {
            _alertRepositoryMock = new Mock<IAlertRepository>();
            _loggerMock = new Mock<ILogger<AlertService>>();
            _alertService = new AlertService(_alertRepositoryMock.Object, _loggerMock.Object);
        }

        #endregion

        #region EvaluateAsync

        [Fact]
        public async Task EvaluateAsync_WhenCpuExceedsThreshold_ShouldTriggerAlert()
        {
            var alert = new Alert
            {
                Id = 1,
                Name = "High CPU",
                MetricType = AlertMetricType.CpuUsage,
                Threshold = 80,
                Condition = AlertCondition.GreaterThan,
                IsEnabled = true
            };

            _alertRepositoryMock.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(new List<Alert> { alert });

            var metrics = new MetricSnapshot { CpuUsagePercent = 95 };
            await _alertService.EvaluateAsync(metrics);

            _alertRepositoryMock.Verify(r => r.AddEventAsync(It.Is<AlertEvent>(e => e.AlertId == alert.Id)), Times.Once);
        }

        [Fact]
        public async Task EvaluateAsync_WhenCpuBelowThreshold_ShouldNotTriggerAlert()
        {
            var alert = new Alert
            {
                Id = 1,
                Name = "High CPU",
                MetricType = AlertMetricType.CpuUsage,
                Threshold = 80,
                Condition = AlertCondition.GreaterThan,
                IsEnabled = true
            };

            _alertRepositoryMock.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(new List<Alert> { alert });

            var metrics = new MetricSnapshot { CpuUsagePercent = 50 };

            await _alertService.EvaluateAsync(metrics);

            _alertRepositoryMock.Verify(r => r.AddEventAsync(It.IsAny<AlertEvent>()), Times.Never);
        }

        [Fact]
        public async Task EvaluateAsync_WithNoActiveAlerts_ShouldNotTriggerAnyAlert()
        {
            _alertRepositoryMock.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(new List<Alert>());

            var metrics = new MetricSnapshot { CpuUsagePercent = 99 };

            await _alertService.EvaluateAsync(metrics);

            _alertRepositoryMock.Verify(r => r.AddEventAsync(It.IsAny<AlertEvent>()), Times.Never);
        }

        [Fact]
        public async Task EvaluateAsync_WhenMemoryBelowThreshold_ShouldTriggerAlert()
        {
            var alert = new Alert
            {
                Id = 1,
                Name = "Low Memory",
                MetricType = AlertMetricType.MemoryUsage,
                Threshold = 20,
                Condition = AlertCondition.LessThan,
                IsEnabled = true
            };

            _alertRepositoryMock.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(new List<Alert> { alert });

            var metrics = new MetricSnapshot
            {
                MemoryUsedBytes = 1000,
                MemoryTotalBytes = 10000
            };

            await _alertService.EvaluateAsync(metrics);
            _alertRepositoryMock.Verify(r => r.AddEventAsync(It.Is<AlertEvent>(e => e.AlertId == alert.Id)), Times.Once);
        }

        #endregion

        #region AddAlertAsync

        [Fact]
        public async Task AddAlertAsync_ShouldCallRepository()
        {
            var alert = new Alert { Name = "Test Alert" };
            await _alertService.AddAlertAsync(alert);
            _alertRepositoryMock.Verify(r => r.AddAsync(alert), Times.Once);
        }

        #endregion

        #region UpdateAlertAsync

        [Fact]
        public async Task UpdateAlertAsync_ShouldCallRepository()
        {
            var alert = new Alert { Id = 1, Name = "Updated Alert" };
            await _alertService.UpdateAlertAsync(alert);
            _alertRepositoryMock.Verify(r => r.UpdateAsync(alert), Times.Once);
        }

        #endregion
    }
}