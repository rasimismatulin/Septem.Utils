using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace Septem.Notifications.Jobs.Factory;

public class DecoratorJobFactory : IJobFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceProvider _serviceProvider;

    public DecoratorJobFactory(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
        _loggerFactory = loggerFactory;
        _serviceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var jobType = bundle.JobDetail.JobType;

        var scope = _serviceProvider.CreateScope();
        var job = scope.ServiceProvider.GetRequiredService(jobType);
        return new JobExecutionExceptionDecorator((IJob)job, _loggerFactory, scope);
    }

    public void ReturnJob(IJob job) => (job as IDisposable)?.Dispose();
}