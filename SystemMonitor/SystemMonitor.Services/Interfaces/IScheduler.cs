namespace SystemMonitor.Services.Interfaces
{
    public interface IScheduler
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync();
    }
}