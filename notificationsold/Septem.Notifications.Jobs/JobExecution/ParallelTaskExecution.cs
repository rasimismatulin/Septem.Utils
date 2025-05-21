using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Septem.Notifications.Jobs.JobExecution;

public static class ParallelTaskExecution
{
    public static Task ParallelForEachAsync<T>(IEnumerable<T> source,
        int degreeOfParallelization,
        Func<T, Task> body)
    {
        async Task AwaitPartition(IEnumerator<T> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    await body(partition.Current);
                }
            }
        }

        return Task.WhenAll(Partitioner.Create(source).GetPartitions(degreeOfParallelization).AsParallel()
            .Select(AwaitPartition));
    }
}