using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Septem.Notifications.Jobs;

namespace Septem.Util.Test.WorkerApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly JobScheduler _jobScheduler;

        public Worker(ILogger<Worker> logger, JobScheduler jobScheduler)
        {
            _logger = logger;
            _jobScheduler = jobScheduler;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _jobScheduler.StartAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}