using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Septem.Notifications.Worker.Config;

namespace Septem.Notifications.Worker.JobExecution;

internal class ConcurrentQueueDispatcher
{
    private readonly ConcurrentQueue<Func<IServiceProvider, Task>> _blockingCollection = new();
    public int LimitCount { get; set; } = JobOptionsBuilder.ConcurrentQueueDispatcherLimit;

    public void Add(IEnumerable<Func<IServiceProvider, Task>> tasks)
    {
        foreach (var task in tasks)
            _blockingCollection.Enqueue(task);
    }

    public ICollection<Func<IServiceProvider, Task>> GetBatch(int batchSize)
    {
        var list = new List<Func<IServiceProvider, Task>>();

        for (int i = 0; i < batchSize; i++)
        {
            var dequeue = _blockingCollection.TryDequeue(out var item);
            if (dequeue)
                list.Add(item);
            else
                return list;
        }

        return list;
    }

    public bool CanAdd => _blockingCollection.Count < LimitCount;
}