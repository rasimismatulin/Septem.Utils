using System;
using Septem.Utils.Helpers.ActionInvoke;

namespace Septem.Utils.MediatR.Features;

public abstract class BaseGetFeature<TResult> : MediatrRequest<TResult>
{
    public override RequestType RequestType => RequestType.Read;

    public Guid Uid { get; set; }

    protected BaseGetFeature(Guid uid)
    {
        Uid = uid;
    }
}