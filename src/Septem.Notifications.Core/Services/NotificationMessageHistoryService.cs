using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Infrastructure;

namespace Septem.Notifications.Core.Services;

internal class NotificationMessageHistoryService : INotificationMessageHistoryService
{
    private readonly NotificationDbContext _notificationDbContext;
    private readonly ILogger _logger;

    public NotificationMessageHistoryService(ILogger<NotificationMessageHistoryService> logger, NotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
        _logger = logger;
    }

    public async Task<ICollection<NotificationMessage>> GetNotificationHistoryAsync(Guid targetUid, NotificationTokenType tokenType, CancellationToken cancellationToken = default)
    {
        var messages = await _notificationDbContext.NotificationMessages
            .AsNoTracking()
            .Where(x => x.NotificationToken.TargetUid == targetUid && x.NotificationToken.Type == tokenType &&
                        (x.Status == NotificationMessageStatus.Success || x.Status == NotificationMessageStatus.Failed))
            .OrderByDescending(x => x.ModifiedDateUtc)
            .Select(x => new NotificationMessage
            {
                MessageUid = x.Uid,
                TokenType = x.NotificationToken.Type,
                Title = x.Title,
                Payload = x.Payload,
                IsSuccess = x.Status == NotificationMessageStatus.Success,
                CreatedDateUtc = x.CreatedDateUtc,
                IsView = x.IsView,
                NotificationUid = x.NotificationUid,
                SentDateUtc = x.ModifiedDateUtc.Value
            })
            .ToListAsync(cancellationToken);

        return messages;
    }

    public async Task<ICollection<NotificationMessage>> GetNotificationHistoryAsync(Guid targetUid, CancellationToken cancellationToken = default)
    {
        var messages = await _notificationDbContext.NotificationMessages
            .AsNoTracking()
            .Where(x => x.NotificationToken.TargetUid == targetUid &&
                        (x.Status == NotificationMessageStatus.Success || x.Status == NotificationMessageStatus.Failed))
            .OrderByDescending(x => x.ModifiedDateUtc)
            .Select(x => new NotificationMessage
            {
                MessageUid = x.Uid,
                TokenType = x.NotificationToken.Type,
                Title = x.Title,
                Payload = x.Payload,
                IsSuccess = x.Status == NotificationMessageStatus.Success,
                CreatedDateUtc = x.CreatedDateUtc,
                IsView = x.IsView,
                NotificationUid = x.NotificationUid,
                SentDateUtc = x.ModifiedDateUtc.Value
            })
            .ToListAsync(cancellationToken);

        return messages;
    }

    public async Task<int> GetNotViewedMessagesCountAsync(Guid targetUid, CancellationToken cancellationToken = default)
    {
        return await _notificationDbContext.NotificationMessages
            .AsNoTracking()
            .Where(x => x.NotificationToken.TargetUid == targetUid && !x.IsView &&
                        (x.Status == NotificationMessageStatus.Success ||
                         x.Status == NotificationMessageStatus.Failed))
            .CountAsync(cancellationToken);
    }

    public async Task SetMessageViewedAsync(Guid messageUid, CancellationToken cancellationToken = default)
    {
        var message = await _notificationDbContext.NotificationMessages
            .Where(x => x.Uid == messageUid)
            .FirstOrDefaultAsync(cancellationToken);

        if (message is { IsView: false })
        {
            message.IsView = true;
            await _notificationDbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Message viewed update: Uid: {message.Uid}");
        }
    }

    public void Dispose()
    {
        _notificationDbContext?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_notificationDbContext != null) await _notificationDbContext.DisposeAsync();
    }
}