using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Windows;
using SystemMonitor.Data;
using SystemMonitor.Data.Interfaces;
using SystemMonitor.Data.Repositories;
using SystemMonitor.Services.Implementations;
using SystemMonitor.Services.Interfaces;

namespace SystemMonitor.UI
{
    public partial class App : Application
    {
        #region Fields

        private ServiceProvider? _serviceProvider;
        private CancellationTokenSource? _cancellationTokenSource;

        #endregion

        #region Startup

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            ApplyMigrations();

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