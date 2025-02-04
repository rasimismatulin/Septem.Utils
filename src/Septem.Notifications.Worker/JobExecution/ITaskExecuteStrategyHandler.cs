using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Septem.Notifications.Worker.JobExecution;

public interface ITaskExecuteStrategyHandler
{
    public bool CanHandle { get; }

    Task Handle(IEnumerable<Func<IServiceProvider, Task>> tasks);
}