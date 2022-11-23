using MediatR;
using Septem.Utils.Helpers.ActionInvoke;
using Septem.Utils.Helpers.ActionInvoke.Collection;

namespace Septem.Utils.MediatR;

public abstract class MediatrRequest : Request, IRequest, IRequest<Result>
{
}

public abstract class MediatrRequest<TResponse> : Request, IRequest<Result<TResponse>>
{
}

public abstract class MediatrRequestOfCollection<TResponse, TSearch> : RequestOfCollection<TSearch>, IRequest<ResultOfCollection<TResponse>>
    where TSearch : CollectionQuery, new()
{
    protected MediatrRequestOfCollection()
    {
        CollectionQuery = new TSearch();
    }

    protected MediatrRequestOfCollection(TSearch query)
    {
        CollectionQuery = query;
    }
}


public abstract class MediatrRequestOfCollection<TResponse> : RequestOfCollection, IRequest<ResultOfCollection<TResponse>>
{
    protected MediatrRequestOfCollection()
    {
    }
}