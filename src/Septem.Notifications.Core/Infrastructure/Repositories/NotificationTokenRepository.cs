using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Infrastructure.Repositories;

internal class NotificationTokenRepository : INotificationTokenRepository
{
    private readonly NotificationDbContext _notificationDbContext;

    public NotificationTokenRepository(NotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
    }

    public IQueryable<NotificationTokenEntity> CollectionQuery => _notificationDbContext.NotificationTokens.Where(x => !x.IsDeleted);

    public async Task AddAsync(NotificationTokenEntity notificationTokenEntity, CancellationToken cancellationToken)
    {
        await _notificationDbContext.NotificationTokens.AddAsync(notificationTokenEntity, cancellationToken);
    }

    public async Task<ICollection<NotificationTokenEntity>> GetByTargetUidAsync(Guid targetUid, NotificationTokenType type, CancellationToken cancellationToken)
    {
        return await _notificationDbContext.NotificationTokens
            .Where(x => !x.IsDeleted && x.TargetUid == targetUid && x.Type == type)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<NotificationTokenEntity>> GetByTokenAsync(string token, NotificationTokenType type, CancellationToken cancellationToken)
    {
        return await _notificationDbContext.NotificationTokens
            .Where(x => !x.IsDeleted && x.Type == type && x.Token == token)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _notificationDbContext.SaveChangesAsync(cancellationToken);
    }
}