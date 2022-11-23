using System.Linq;
using Septem.Utils.Helpers.ActionInvoke;

namespace Septem.Utils.MediatR.Features;

public abstract class BaseGetCollectionQueryFeature<TResult> : MediatrRequest<IQueryable<TResult>>
{
    public override RequestType RequestType => RequestType.Read;

    protected BaseGetCollectionQueryFeature()
    {
    }
}