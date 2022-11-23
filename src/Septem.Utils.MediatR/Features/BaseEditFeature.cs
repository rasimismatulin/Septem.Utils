using System;
using Septem.Utils.Helpers.ActionInvoke;

namespace Septem.Utils.MediatR.Features;

public abstract class BaseEditFeature : MediatrRequest
{
    public override RequestType RequestType => RequestType.Edit;

    public Guid Uid { get; set; }

    protected BaseEditFeature()
    {
        
    }

    protected BaseEditFeature(Guid uid)
    {
        Uid = uid;
    }
}