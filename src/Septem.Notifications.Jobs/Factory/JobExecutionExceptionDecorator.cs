using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Septem.Notifications.Jobs.Factory;

public class JobExecutionExceptionDecorator : IJob, IDisposable
{
    public const string JobsGroup = "Septem.Notifications.Jobs";
    private readonly IJob _innerJob;
    private readonly IServiceScope _scope;
    private readonly ILogger _logger;

    protected Action<IJobExecutionContext> OnCatch { get; set; }

    public JobExecutionExceptionDecorator(IJob innerJob, ILoggerFactory loggerFactory, IServiceScope scope)
    {
        _innerJob = innerJob;
        _scope = scope;
        _logger = loggerFactory.CreateLogger<JobExecutionExceptionDecorator>();
    }


    async Task IJob.Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogTrace($"Execute started. Detail: [{context.JobDetail.JobType.Name}]");
            await _innerJob.Execute(context);
            _logger.LogTrace($"Execute ended. Detail: [{context.JobDetail.JobType.Name}]");
        }
        catch (Exception ex)
        {
            var exceptionMessages = new StringBuilder();
            var innerException = ex;
            while (innerException != null)
            {
                exceptionMessages.AppendLine(innerException.Message);
                innerException = innerException.InnerException;
            }
            _logger.LogError($"Exception during executing job. Detail: [{context.JobDetail.JobType.Name}] Message:[{exceptionMessages}]", ex);
            OnCatch?.Invoke(context);
        }
    }

    void IDisposable.Dispose()
    {
        var disposableJob = _innerJob as IDisposable;
        disposableJob?.Dispose();

        _scope.Dispose();
    }
}