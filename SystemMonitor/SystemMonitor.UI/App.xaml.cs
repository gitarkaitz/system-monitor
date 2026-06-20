using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SystemMonitor.Data;
using SystemMonitor.Data.Interfaces;
using SystemMonitor.Data.Models;
using SystemMonitor.Data.Repositories;
using SystemMonitor.Services.Implementations;
using SystemMonitor.Services.Interfaces;
using SystemMonitor.UI.ViewModels;

namespace SystemMonitor.UI
{
    public partial class App : Application
    {
        #region Fields

        private ServiceProvider? _serviceProvider;
        private CancellationTokenSource? _cancellationTokenSource;

        #endregion

        #region Startup

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            ApplyMigrations();
            await SeedAlertsAsync();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            var scheduler = _serviceProvider.GetRequiredService<IScheduler>();
            _cancellationTokenSource = new CancellationTokenSource();
            _ = scheduler.StartAsync(_cancellationTokenSource.Token);
        }

        #endregion

        #region Configuration

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                { 
                    options.UseSqlite("DataSource=systemmonitor.db"); 
                });

            services.AddScoped<IMetricSnapshotRepository, MetricSnapshotRepository>();
            services.AddScoped<IProcessSnapshotRepository, ProcessSnapshotRepository>();
            services.AddScoped<IAlertRepository, AlertRepository>();

            services.AddLogging(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            services.AddScoped<IProcessService, ProcessService>();
            services.AddScoped<IMetricsService, MetricsService>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddSingleton<IScheduler, Scheduler>();

            services.AddTransient<MainWindow>();
            services.AddTransient<MainViewModel>();
        }

        private void ApplyMigrations()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
        }

        private async Task SeedAlertsAsync()
        {
            if (_serviceProvider == null)
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();

            var existing = await repository.GetAllActiveAsync();
            if (existing.Any())
            {
                return;
            }

            var defaultAlerts = new List<Alert>
            {
                new() { Name = "CPU alta", MetricType = AlertMetricType.CpuUsage, Threshold = 85, Condition = AlertCondition.GreaterThan, IsEnabled = true },
                new() { Name = "Memoria alta", MetricType = AlertMetricType.MemoryUsage, Threshold = 90, Condition = AlertCondition.GreaterThan, IsEnabled = true },
                new() { Name = "Red recibida baja", MetricType = AlertMetricType.NetworkReceived, Threshold = 10_485_760, Condition = AlertCondition.LessThan, IsEnabled = true },
                new() { Name = "Red enviada baja", MetricType = AlertMetricType.NetworkSent, Threshold = 5_242_880, Condition = AlertCondition.LessThan, IsEnabled = true },
                new() { Name = "Disco leído alto", MetricType = AlertMetricType.DiskRead, Threshold = 104_857_600, Condition = AlertCondition.GreaterThan, IsEnabled = true },
                new() { Name = "Disco escrito alto", MetricType = AlertMetricType.DiskWrite, Threshold = 104_857_600, Condition = AlertCondition.GreaterThan, IsEnabled = true }
            };

            foreach (var alert in defaultAlerts)
            {
                await repository.AddAsync(alert);
            }
        }

        #endregion

        #region Shutdown

        protected override void OnExit(ExitEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        #endregion
    }
}