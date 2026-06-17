namespace SystemMonitor.Services.Interfaces
{
    public interface IScheduler
    {
        event EventHandler<SchedulerTickEventArgs>? OnTick;
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync();
    }
}