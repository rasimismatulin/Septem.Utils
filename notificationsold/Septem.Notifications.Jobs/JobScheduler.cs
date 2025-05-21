using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Septem.Notifications.Jobs.Config;
using Septem.Notifications.Jobs.Factory;
using Septem.Notifications.Jobs.JobExecution;

namespace Septem.Notifications.Jobs;

public class JobScheduler
{
    private readonly DecoratorJobFactory _jobFactory;
    private readonly ILogger _logger;
    private static TriggerKey ReceiverCreateInitializationJobTriggerKey => new(nameof(ReceiverCreateInitializationJob), JobExecutionExceptionDecorator.JobsGroup);
    private static TriggerKey MessageCreateInitializationJobTriggerKey => new(nameof(MessageCreateInitializationJob), JobExecutionExceptionDecorator.JobsGroup);
    private static TriggerKey MessageSendInitializationJobTriggerKey => new(nameof(MessageSendInitializationJob), JobExecutionExceptionDecorator.JobsGroup);
    private static TriggerKey ConcurrentQueueDispatcherJobTriggerKey => new(nameof(ConcurrentQueueDispatcherJob), JobExecutionExceptionDecorator.JobsGroup);
    private static readonly ICollection<TriggerKey> CustomTriggers = new List<TriggerKey>();


    public JobScheduler(ILoggerFactory loggerFactory, DecoratorJobFactory jobFactory)
    {
        _jobFactory = jobFactory;
        _logger = loggerFactory.CreateLogger<JobScheduler>();
    }

    public async Task StartAsync()
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        scheduler.JobFactory = _jobFactory;

        await StartJobAsync<ReceiverCreateInitializationJob>(scheduler, ReceiverCreateInitializationJobTriggerKey);
        await StartJobAsync<MessageCreateInitializationJob>(scheduler, MessageCreateInitializationJobTriggerKey);
        await StartJobAsync<MessageSendInitializationJob>(scheduler, MessageSendInitializationJobTriggerKey);
        await StartJobAsync<ConcurrentQueueDispatcherJob>(scheduler, ConcurrentQueueDispatcherJobTriggerKey);

        foreach (var job in JobOptionsBuilder.CustomJobs)
        {
            var trigger = new TriggerKey($"custom_{job.JobType.Name}", JobExecutionExceptionDecorator.JobsGroup);
            CustomTriggers.Add(trigger);
            _logger.LogInformation($"Custom job created: {job.JobType.FullName}");
            await StartJobAsync(scheduler, job.JobType, job.Interval, trigger);
        }

        await scheduler.Start();
    }


    public async Task StopAsync()
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.UnscheduleJob(ReceiverCreateInitializationJobTriggerKey);
        await scheduler.UnscheduleJob(MessageCreateInitializationJobTriggerKey);
        await scheduler.UnscheduleJob(MessageSendInitializationJobTriggerKey);
        await scheduler.UnscheduleJob(ConcurrentQueueDispatcherJobTriggerKey);
        foreach (var customTrigger in CustomTriggers)
        {
            await scheduler.UnscheduleJob(customTrigger);
        }
    }

    private async Task StartJobAsync(IScheduler scheduler, Type jobType, double jobInterval, TriggerKey triggerKey)
    {
        var jobDetail =
            JobBuilder.Create(jobType)
                .WithIdentity(jobType.Name, JobExecutionExceptionDecorator.JobsGroup).Build();

        var jobTrigger = TriggerBuilder.Create().ForJob(jobDetail)
            .WithIdentity(triggerKey)
            .StartNow()
            .WithSimpleSchedule(sh => sh.WithInterval(TimeSpan.FromMilliseconds(jobInterval))
                .RepeatForever())
            .Build();

        var times = TriggerUtils.ComputeFireTimes((IOperableTrigger)jobTrigger, null, 10);
        for (var i = 0; i < times.Count; i++)
            _logger.LogInformation($"{i + 1}. {jobType.Name} computed Fire time: {times[i]}");

        await scheduler.ScheduleJob(jobDetail, jobTrigger);
    }


    private async Task StartJobAsync<T>(IScheduler scheduler, TriggerKey triggerKey)
        where T : IJob
    {
        var jobInterval = JobOptionsBuilder.GetByName(typeof(T).Name);

        var jobDetail =
            JobBuilder.Create<T>()
                .WithIdentity(typeof(T).Name, JobExecutionExceptionDecorator.JobsGroup).Build();

        var jobTrigger = TriggerBuilder.Create().ForJob(jobDetail)
            .WithIdentity(triggerKey)
            .StartNow()
            .WithSimpleSchedule(sh => sh.WithInterval(TimeSpan.FromMilliseconds(jobInterval))
                .RepeatForever())
            .Build();

        var times = TriggerUtils.ComputeFireTimes((IOperableTrigger)jobTrigger, null, 10);
        for (var i = 0; i < times.Count; i++)
            _logger.LogInformation($"{i + 1}. {typeof(T).Name} computed Fire time: {times[i]}");

        await scheduler.ScheduleJob(jobDetail, jobTrigger);
    }

}