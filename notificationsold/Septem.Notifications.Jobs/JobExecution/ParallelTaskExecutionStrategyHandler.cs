using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Septem.Notifications.Jobs.Config;

namespace Septem.Notifications.Jobs.JobExecution;

internal class ParallelTaskExecutionStrategyHandler : ITaskExecuteStrategyHandler
{
    private readonly IServiceProvider _serviceProvider;
    public bool CanHandle => true;

    public ParallelTaskExecutionStrategyHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(IEnumerable<Func<IServiceProvider, Task>> tasks)
    {
        await ParallelTaskExecution.ParallelForEachAsync(tasks, JobOptionsBuilder.DegreeOfParallelization, async func =>
        {
            await func(_serviceProvider);
        });
    }
}