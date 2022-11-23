using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Septem.Utils.Helpers.ActionInvoke;
using Septem.Utils.Helpers.ActionInvoke.Collection;

namespace Septem.Utils.MediatR;

public static class MediatrExtensions
{
    public static async Task<Result> SendRequestAsync(this IMediator mediator, MediatrRequest request, CancellationToken cancellationToken) =>
        await mediator.Send<Result>(request, cancellationToken).ConfigureAwait(false);

    public static async Task<Result<T>> SendRequestAsync<T>(this IMediator mediator, MediatrRequest<T> request, CancellationToken cancellationToken) =>
        await mediator.Send<Result<T>>(request, cancellationToken).ConfigureAwait(false);

    public static async Task<ResultOfCollection<T>> SendRequestAsync<T, TSearch>(this IMediator mediator, MediatrRequestOfCollection<T, TSearch> request, CancellationToken cancellationToken)
        where TSearch : CollectionQuery, new() =>
        await mediator.Send<ResultOfCollection<T>>(request, cancellationToken).ConfigureAwait(false);

    public static async Task<ResultOfCollection<T>> SendRequestAsync<T>(this IMediator mediator, MediatrRequestOfCollection<T> request, CancellationToken cancellationToken) =>
        await mediator.Send<ResultOfCollection<T>>(request, cancellationToken).ConfigureAwait(false);
}