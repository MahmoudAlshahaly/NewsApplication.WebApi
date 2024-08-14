using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shipping.BLL.Managers;

namespace NewsApplication.BLL.Services
{
    public class NewsCleanUpService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        public NewsCleanUpService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Run every one minute
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); 
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        private void DoWork(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var newsManager = scope.ServiceProvider.GetRequiredService<INewsManager>();
                // Call the method to soft delete expired news
                newsManager.SoftDeleteExpiredNewsAsync(DateTime.Now).Wait();
            }
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
