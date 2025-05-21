using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Septem.Notifications.Jobs.Config;

namespace Septem.Notifications.Jobs.JobExecution;

internal class ConcurrentQueueDispatcherJobStrategyHandler : ITaskExecuteStrategyHandler
{
    private readonly ConcurrentQueueDispatcher _concurrentQueueDispatcher;

    public ConcurrentQueueDispatcherJobStrategyHandler(ConcurrentQueueDispatcher concurrentQueueDispatcher)
    {
        _concurrentQueueDispatcher = concurrentQueueDispatcher;
    }

    public bool CanHandle => _concurrentQueueDispatcher.CanAdd;

    public Task Handle(IEnumerable<Func<IServiceProvider, Task>> tasks)
    {
        _concurrentQueueDispatcher.Add(tasks);
        return Task.CompletedTask;
    }
}