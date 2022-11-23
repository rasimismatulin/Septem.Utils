using Septem.Utils.Helpers.ActionInvoke;

namespace Septem.Utils.MediatR.Features;

public abstract class BaseCreateFeature<TResult> : MediatrRequest<TResult>
{
    public override RequestType RequestType => RequestType.Create;
    protected BaseCreateFeature()
    {

    }
}