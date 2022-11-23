using Septem.Utils.Helpers.ActionInvoke;
using Septem.Utils.Helpers.ActionInvoke.Collection;

namespace Septem.Utils.MediatR.Features;

public abstract class BaseGetCollectionFeature<TResult> : MediatrRequestOfCollection<TResult>
{
    public override RequestType RequestType => RequestType.Read;

    protected BaseGetCollectionFeature()
    {
    }
}


public abstract class BaseGetCollectionFeature<TResult, TSearch> : MediatrRequestOfCollection<TResult, TSearch>
    where TSearch : CollectionQuery, new()
{
    public override RequestType RequestType => RequestType.Read;

    protected BaseGetCollectionFeature()
    {
    }

    protected BaseGetCollectionFeature(TSearch query) : base(query)
    {
    }
}