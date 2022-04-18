using System;

namespace Septem.Notifications.Core.Exceptions;

internal class ServiceNotImplementedException<T> : Exception
{
    public ServiceNotImplementedException() : base($"{typeof(T).FullName} not implemented")
    {

    }
}