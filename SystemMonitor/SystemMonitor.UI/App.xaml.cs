using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SystemMonitor.Data;
using SystemMonitor.Data.Interfaces;
using SystemMonitor.Data.Repositories;
using System.Windows;

namespace SystemMonitor.UI
{
    public partial class App : Application
    {
        #region Fields

        private ServiceProvider? _serviceProvider;

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

            services.AddTransient<MainWindow>();
        }

        private void ApplyMigrations()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
        }

        #endregion

        #region Shutdown

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        #endregion
    }
}