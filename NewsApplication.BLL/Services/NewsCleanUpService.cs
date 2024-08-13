using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shipping.BLL.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // Run every 24 hours
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
                newsManager.SoftDeleteExpiredNewsAsync(DateTime.Now).Wait();// Call the method to soft delete expired news
            }
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        await _newsManager.SoftDeleteExpiredNewsAsync(DateTime.Now);
        //        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Run daily
        //    }
        //}
    }

}
