using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Septem.Utils.Helpers.ActionInvoke;
using Septem.Utils.Helpers.Extensions;
using Septem.Utils.Helpers.JsonConverters;

namespace Septem.Utils.MediatR;

public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : Request, IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger _log;

    public ExceptionHandlingBehavior(ILoggerFactory loggerFactory)
    {
        if (loggerFactory == null)
            throw new ArgumentNullException(nameof(loggerFactory));
        _log = loggerFactory.CreateLogger(nameof(TRequest));
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            var str = "LogRequestJson OFF";
            if (MediatrConfiguration.LogRequestJson)
                str = JsonSerializer.Serialize(request, JsonSerializerOption.Default);

            _log.LogInformation("Execute: [" + request.GetType().FullName + "] " + str);
            return await next().ConfigureAwait(false);
        }
        catch (ControversyException ex)
        {
            _log.LogWarning(ex, $"ControversyException on Request: Id = {request.Id}");
            var issue = new Issue(IssueOrigin.Controversy, ex.GetMessages());
            return Activator.CreateInstance(typeof(TResponse), request.Id, issue) as TResponse;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, $"ExceptionHandlingBehavior on Request: Id = {request.Id}");
            var issue = ex.Source != null && (ex.Source.Contains("Infrastructure") || ex.Source.Contains("Npgsql")) ?
                new Issue(IssueOrigin.ExternalFailure, ex.GetMessages()) :
                new Issue(IssueOrigin.Unknown, ex.GetMessages());

            return Activator.CreateInstance(typeof(TResponse), request.Id, issue) as TResponse;
        }
    }
}