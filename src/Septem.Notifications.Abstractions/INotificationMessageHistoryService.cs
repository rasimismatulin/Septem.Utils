using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Septem.Notifications.Abstractions;

public interface INotificationMessageHistoryService
{
    Task<ICollection<NotificationMessage>> GetNotificationHistoryAsync(Guid targetUid, NotificationTokenType tokenType, CancellationToken cancellationToken = default);

    Task<ICollection<NotificationMessage>> GetNotificationHistoryAsync(Guid targetUid, CancellationToken cancellationToken = default);

    Task<int> GetNotViewedMessagesCountAsync(Guid targetUid, CancellationToken cancellationToken = default);

    Task SetMessageViewedAsync(Guid messageUid, CancellationToken cancellationToken = default);
}