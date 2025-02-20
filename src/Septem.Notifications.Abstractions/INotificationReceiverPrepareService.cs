using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Septem.Notifications.Abstractions;

public interface INotificationReceiverPrepareService : IDisposable, IAsyncDisposable
{
    Task<ICollection<Receiver>> PrepareReceiverAsync(Receiver receiver, CancellationToken cancellationToken);
}