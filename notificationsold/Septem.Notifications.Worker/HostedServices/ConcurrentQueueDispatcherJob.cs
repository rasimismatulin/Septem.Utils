using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Septem.Notifications.Worker.Config;
using Septem.Notifications.Worker.JobExecution;

namespace Septem.Notifications.Worker.HostedServices;

internal class ConcurrentQueueDispatcherJob : BaseHostedService
{
    private readonly ConcurrentQueueDispatcher _concurrentCollectionDispatcher;
    private readonly IServiceProvider _serviceProvider;


    public ConcurrentQueueDispatcherJob(IServiceProvider serviceProvider, ILogger<ConcurrentQueueDispatcherJob> logger,
        ConcurrentQueueDispatcher concurrentCollectionDispatcher)
        : base(serviceProvider, logger)
    {
        _concurrentCollectionDispatcher = concurrentCollectionDispatcher;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteInternalAsync(CancellationToken stoppingToken)
    {
        var tasks = _concurrentCollectionDispatcher.GetBatch(JobOptionsBuilder.ConcurrentQueueDispatcherJobBatchSize);
        if (tasks.Any())
        {
            await ParallelTaskExecution.ParallelForEachAsync(tasks, JobOptionsBuilder.DegreeOfParallelization, async func => { await func(_serviceProvider); });
        }
    }
}