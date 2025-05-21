using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Septem.Notifications.Abstractions;

public interface INotificationService : IDisposable, IAsyncDisposable
{
    Task CreateNotificationAsync(Notification notification, Receiver receiver, CancellationToken cancellationToken = default);

    Task CreateNotificationAsync(Notification notification, ICollection<Receiver> receivers, CancellationToken cancellationToken = default);

    Task CancelAsync(Guid groupKey, CancellationToken cancellationToken = default);

    Task CancelAsync(string cancellationKey, CancellationToken cancellationToken = default);

    Task CancelByUidAsync(Guid uid, CancellationToken cancellationToken = default);
}