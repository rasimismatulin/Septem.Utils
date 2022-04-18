using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Infrastructure;

internal interface INotificationTokenRepository
{
    IQueryable<NotificationTokenEntity> CollectionQuery { get; }

    Task<ICollection<NotificationTokenEntity>> GetByTargetUidAsync(Guid targetUid, NotificationTokenType type, CancellationToken cancellationToken);

    Task<ICollection<NotificationTokenEntity>> GetByTokenAsync(string token, NotificationTokenType type, CancellationToken cancellationToken);

    Task AddAsync(NotificationTokenEntity notificationTokenEntity, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}