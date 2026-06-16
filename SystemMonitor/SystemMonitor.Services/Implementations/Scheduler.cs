using Microsoft.Extensions.Logging;
using SystemMonitor.Data.Interfaces;
using SystemMonitor.Services.Interfaces;

namespace SystemMonitor.Services.Implementations
{
    public class Scheduler : IScheduler
    {
        #region Fields

        private readonly IMetricsService _metricsService;
        private readonly IProcessService _processService;
        private readonly IAlertService _alertService;
        private readonly IMetricSnapshotRepository _metricRepository;
        private readonly IProcessSnapshotRepository _processRepository;
        private readonly ILogger<Scheduler> _logger;
        private PeriodicTimer? _timer;

        #endregion

        #region Constructor

        public Scheduler(IMetricsService metricsService, IProcessService processService, IAlertService alertService,
            IMetricSnapshotRepository metricRepository, IProcessSnapshotRepository processRepository, ILogger<Scheduler> logger)
        {
            _metricsService = metricsService;
            _processService = processService;
            _alertService = alertService;
            _metricRepository = metricRepository;
            _processRepository = processRepository;
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler started");
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await _timer.WaitForNextTickAsync(cancellationToken))
            {
                await TickAsync();
            }
        }

        public Task StopAsync()
        {
            _timer?.Dispose();
            _logger.LogInformation("Scheduler stopped");
            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        private async Task TickAsync()
        {
            try
            {
                var metrics = await _metricsService.GetCurrentMetricsAsync();
                await _metricRepository.AddAsync(metrics);

                var processes = await _processService.GetCurrentProcessesAsync();
                await _processRepository.AddRangeAsync(processes);

                await _alertService.EvaluateAsync(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during scheduler tick");
            }
        }

        #endregion
    }
}