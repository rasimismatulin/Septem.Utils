using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Septem.Notifications.Jobs.JobExecution;

namespace Septem.Notifications.Jobs.Config;

public class JobOptionsBuilder
{
    private static readonly IDictionary<string, double> IntervalDictionary = new Dictionary<string, double>();
    private const double DefaultJobInterval = 1000;
    public static int MessageCreateJobBatchSize = 500;
    public static int MessageSendJobBatchSize = 500;
    public static int ReceiverCreateJobBatchSize = 500;
    public static int DegreeOfParallelization = 4;
    public static int ConcurrentQueueDispatcherLimit = 5000;
    public static int ConcurrentQueueDispatcherJobBatchSize = 1000;

    public void SetMessageCreateInitializationJobInterval(double millisecond) =>
        IntervalDictionary.Add("MessageCreateInitializationJob", millisecond);

    public void SetMessageSendInitializationJobInterval(double millisecond) =>
        IntervalDictionary.Add("MessageSendInitializationJob", millisecond);

    public void SetReceiverCreateInitializationJobInterval(double millisecond) =>
        IntervalDictionary.Add("ReceiverCreateInitializationJob", millisecond);

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


    public Type TaskExecuteStrategyHandler = typeof(ConcurrentQueueDispatcherJobStrategyHandler);


    internal static ICollection<JobInformation> CustomJobs = new List<JobInformation>();

    public void AddCustomJob<T>(T job, double interval)
    {
        CustomJobs.Add(new JobInformation(job.GetType(), interval));
    }

    public void Build(IServiceCollection services)
    {
        services.AddScoped(typeof(ITaskExecuteStrategyHandler), TaskExecuteStrategyHandler);
    }


    public static double GetByName(string name)
    {
        return IntervalDictionary.ContainsKey(name) ? IntervalDictionary[name] : DefaultJobInterval;
    }
}