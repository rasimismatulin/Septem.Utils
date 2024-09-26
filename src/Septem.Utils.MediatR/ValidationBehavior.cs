using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Septem.Utils.Helpers.ActionInvoke;

namespace Septem.Utils.MediatR;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : Request, IRequest<TResponse>
    where TResponse : Result
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validator = _serviceProvider.GetService<IValidator<TRequest>>();
        if (validator == null)
            return await next().ConfigureAwait(false);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
            return await next().ConfigureAwait(false);

        var array = validationResult.Errors.Select(e => new { e.ErrorCode, e.ErrorMessage }).ToArray();
        var origin = IssueOrigin.UserInput;

        if (array.Any(x => x.ErrorCode == "Controversy"))
            origin = IssueOrigin.Controversy;

        if (array.Any(x => x.ErrorCode == "NotFound"))
            origin = IssueOrigin.NotFound;

        var issue = new Issue(origin, array.Select(x => x.ErrorMessage).Distinct().ToArray());
        return Activator.CreateInstance(typeof(TResponse), request.Id, issue) as TResponse;
    }
}