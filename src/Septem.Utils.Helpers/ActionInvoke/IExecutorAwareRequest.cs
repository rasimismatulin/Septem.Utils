using System;

namespace Septem.Utils.Helpers.ActionInvoke;

public interface IExecutorAwareRequest
{
    Guid ExecutorUid { set; }
}