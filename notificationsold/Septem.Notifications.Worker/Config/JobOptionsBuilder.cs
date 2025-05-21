using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Septem.Notifications.Worker.HostedServices;
using Septem.Notifications.Worker.JobExecution;

namespace Septem.Notifications.Worker.Config;

public class JobOptionsBuilder
{
    private static readonly IDictionary<string, int> IntervalDictionary = new Dictionary<string, int>();
    private const int DefaultJobInterval = 1000;
    public static int MessageCreateJobBatchSize = 500;
    public static int MessageSendJobBatchSize = 500;
    public static int ReceiverCreateJobBatchSize = 500;
    public static int DegreeOfParallelization = 4;
    public static int ConcurrentQueueDispatcherLimit = 5000;
    public static int ConcurrentQueueDispatcherJobBatchSize = 1000;

    public void SetMessageCreateInitializationJobInterval(int millisecond) =>
        IntervalDictionary.Add(nameof(MessageCreateInitializationJob), millisecond);

    public void SetMessageSendInitializationJobInterval(int millisecond) =>
        IntervalDictionary.Add(nameof(MessageSendInitializationJob), millisecond);

    public void SetReceiverCreateInitializationJobInterval(int millisecond) =>
        IntervalDictionary.Add(nameof(ReceiverCreateInitializationJob), millisecond);

    public void SetMessageCreateJobBatchSize(int batchSize) =>
        MessageCreateJobBatchSize = batchSize;
    public void SetMessageSendInitializationJob(int batchSize) =>
        MessageSendJobBatchSize = batchSize;
    public void ReceiverCreateInitializationJob(int batchSize) =>
        ReceiverCreateJobBatchSize = batchSize;


    public void UseConcurrentQueueDispatcherJobStrategyHandler(int degreeOfParallelization = 4, int concurrentQueueDispatcherLimit = 5000, int concurrentQueueDispatcherJobBatchSize = 1000)
    {
        TaskExecuteStrategyHandler = typeof(ConcurrentQueueDispatcherJobStrategyHandler);
        DegreeOfParallelization = degreeOfParallelization;
        ConcurrentQueueDispatcherLimit = concurrentQueueDispatcherLimit;
        ConcurrentQueueDispatcherJobBatchSize = concurrentQueueDispatcherJobBatchSize;
    }

    public void UseParallelTaskExecutionStrategyHandler(int degreeOfParallelization = 4)
    {
        TaskExecuteStrategyHandler = typeof(ParallelTaskExecutionStrategyHandler);
        DegreeOfParallelization = degreeOfParallelization;
    }
    public void UseSynchronousTaskExecutionStrategyHandler() =>
        TaskExecuteStrategyHandler = typeof(SynchronousTaskExecutionStrategyHandler);


    public Type TaskExecuteStrategyHandler = typeof(ParallelTaskExecutionStrategyHandler);


    internal static ICollection<JobInformation> CustomJobs = new List<JobInformation>();

    public void AddCustomJob<T>(T job, double interval)
    {
        CustomJobs.Add(new JobInformation(job.GetType(), interval));
    }

    public void Build(IServiceCollection services)
    {
        services.AddTransient(typeof(ITaskExecuteStrategyHandler), TaskExecuteStrategyHandler);
    }


    public static int GetIntervalByName(string name)
    {
        return IntervalDictionary.TryGetValue(name, out var value) ? value : DefaultJobInterval;
    }
}