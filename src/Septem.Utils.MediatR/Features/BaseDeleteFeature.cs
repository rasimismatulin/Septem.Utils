using System;
using System.Collections.Generic;
using Septem.Utils.Helpers.ActionInvoke;

namespace Septem.Utils.MediatR.Features;

public abstract class BaseDeleteFeature : MediatrRequest
{
    public override RequestType RequestType => RequestType.Delete;

    public ICollection<Guid> Uid { get; set; }

    protected BaseDeleteFeature(Guid uid)
    {
        Uid = new[] { uid };
    }

    protected BaseDeleteFeature(ICollection<Guid> uid)
    {
        Uid = uid;
    }
}