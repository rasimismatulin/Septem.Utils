using System;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Septem.Notifications.Jobs.Config;

namespace Septem.Notifications.Jobs.JobExecution;

[DisallowConcurrentExecution]
internal class ConcurrentQueueDispatcherJob : IJob
{
    private readonly ConcurrentQueueDispatcher _concurrentCollectionDispatcher;
    private readonly IServiceProvider _serviceProvider;

    public ConcurrentQueueDispatcherJob(ConcurrentQueueDispatcher concurrentCollectionDispatcher,
        IServiceProvider serviceProvider)
    {
        _concurrentCollectionDispatcher = concurrentCollectionDispatcher;
        _serviceProvider = serviceProvider;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        while (true)
        {
            var tasks = _concurrentCollectionDispatcher.GetBatch(JobOptionsBuilder.ConcurrentQueueDispatcherJobBatchSize);
            if (tasks.Any())
            {
                await ParallelTaskExecution.ParallelForEachAsync(tasks, JobOptionsBuilder.DegreeOfParallelization, async func => { await func(_serviceProvider); });
            }
        }
    }
}