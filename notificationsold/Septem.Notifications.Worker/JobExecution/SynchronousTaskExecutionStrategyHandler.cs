﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Septem.Notifications.Worker.JobExecution;

internal class SynchronousTaskExecutionStrategyHandler : ITaskExecuteStrategyHandler
{
    private readonly IServiceProvider _serviceProvider;
    public bool CanHandle => true;

    public SynchronousTaskExecutionStrategyHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(IEnumerable<Func<IServiceProvider, Task>> tasks)
    {
        foreach (var task in tasks)
        {
            await task(_serviceProvider);
        }
    }
}