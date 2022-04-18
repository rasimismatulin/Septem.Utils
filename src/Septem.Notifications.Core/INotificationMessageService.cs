using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Septem.Notifications.Core;

public interface INotificationMessageService
{
    Task<ICollection<Guid>> GetPendingNotificationsUidAsync(int batchSize, CancellationToken cancellationToken = default);
    Task CreateNotificationReceiversAsync(Guid notificationUid, CancellationToken cancellationToken = default);

    Task<ICollection<Guid>> GetNextNotificationsUidAsync(int batchSize, CancellationToken cancellationToken = default);
    Task CreateNotificationMessagesAsync(Guid notificationUid, CancellationToken cancellationToken = default);

    Task<ICollection<Guid>> GetNextMessagesUidAsync(int batchSize, CancellationToken cancellationToken = default);
    Task SendMessagesAsync(Guid messageUid, CancellationToken cancellationToken = default);
}