using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Septem.Notifications.Worker.Config;

namespace Septem.Notifications.Worker.HostedServices;

internal abstract class BaseHostedService : BackgroundService
{
    protected readonly ILogger Logger;
    protected readonly IServiceProvider ServiceProvider;
    private readonly string _serviceName;
    private bool _disableDelayAfterCurrentExecution;

    protected BaseHostedService(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger(GetType());
        _serviceName = GetType().Name;
        ServiceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteInternalAsync(stoppingToken);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during ExecuteAsync in {serviceType}", _serviceName);
            }
            finally
            {
                if (_disableDelayAfterCurrentExecution)
                    _disableDelayAfterCurrentExecution = false;
                else
                    await Task.Delay(JobOptionsBuilder.GetIntervalByName(_serviceName), stoppingToken);
            }
        }
    }

    protected abstract Task ExecuteInternalAsync(CancellationToken stoppingToken);
    protected void DisableDelayAfterCurrentExecution() => _disableDelayAfterCurrentExecution = true;
}