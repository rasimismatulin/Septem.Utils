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

internal class NotificationTokenService : INotificationTokenService
{
    private readonly NotificationDbContext _notificationDbContext;
    private readonly ILogger _logger;

    public NotificationTokenService(ILoggerFactory loggerFactory, NotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
        _logger = loggerFactory.CreateLogger<NotificationTokenService>();
    }

    public async Task<ICollection<NotificationToken>> GetAsync(Guid targetUid, CancellationToken cancellationToken = default)
    {
        var entities = await _notificationDbContext.NotificationTokens
            .AsNoTracking()
            .Where(x => x.TargetUid == targetUid)
            .ToListAsync(cancellationToken);

        return entities.Select(Converter.GetNotificationToken).ToList();
    }

    public async Task<ICollection<NotificationToken>> GetAsync(Guid targetUid, NotificationTokenType type, CancellationToken cancellationToken = default)
    {
        var entities = await _notificationDbContext.NotificationTokens
            .AsNoTracking()
            .Where(x => x.TargetUid == targetUid &&
                        x.Type == type)
            .ToListAsync(cancellationToken);

        return entities.Select(Converter.GetNotificationToken).ToList();
    }

    public async Task<NotificationToken> GetAsync(Guid targetUid, NotificationTokenType type, string deviceUid, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationDbContext.NotificationTokens
            .AsNoTracking()
            .Where(x => x.TargetUid == targetUid &&
                        x.Type == type &&
                        x.DeviceId == deviceUid)
            .FirstOrDefaultAsync(cancellationToken);

        return Converter.GetNotificationToken(entity);
    }

    public async Task SaveAsync(NotificationToken notificationToken, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationDbContext.NotificationTokens
             .Where(x => x.TargetUid == notificationToken.TargetUid &&
                         x.Type == notificationToken.Type &&
                         x.DeviceId == notificationToken.DeviceId)
             .FirstOrDefaultAsync(cancellationToken);

        if (entity == default)
        {
            entity = Converter.GetNotificationTokenEntity(notificationToken);
            _notificationDbContext.NotificationTokens.Add(entity);
        }
        else
        {
            if (entity.Token != notificationToken.Token)
            {
                entity.ModifiedDateUtc = DateTimeOffset.UtcNow;
                entity.Token = notificationToken.Token;
            }

            if (!string.IsNullOrWhiteSpace(notificationToken.Language))
            {
                if (entity.Language != notificationToken.Language)
                {
                    entity.ModifiedDateUtc = DateTimeOffset.UtcNow;
                    entity.Language = notificationToken.Language;
                }
            }
        }

        await _notificationDbContext.SaveChangesAsync(cancellationToken);
        notificationToken.Uid = entity.Uid;
        _logger.LogInformation($"Notification token saved: Uid: {notificationToken.Uid}; TargetUid: {notificationToken.TargetUid};");
    }

    public async Task SaveLanguageAsync(string language, Guid targetUid, NotificationTokenType tokenType, string deviceId, CancellationToken cancellationToken = default)
    {
        var entities = await _notificationDbContext.NotificationTokens
            .Where(x => x.TargetUid == targetUid &&
                        x.Type == tokenType &&
                        x.DeviceId == deviceId)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.Language = language;
            _logger.LogInformation($"Notification token language saved: Uid: {entity.Uid}; TargetUid: {targetUid}; Language: {language}");
        }

        await _notificationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveLanguageAsync(string language, Guid targetUid, NotificationTokenType tokenType, CancellationToken cancellationToken = default)
    {
        var entities = await _notificationDbContext.NotificationTokens
            .Where(x => x.TargetUid == targetUid &&
                        x.Type == tokenType)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.Language = language;
            _logger.LogInformation($"Notification token language saved: Uid: {entity.Uid}; TargetUid: {targetUid}; Language: {language}");
        }

        await _notificationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveLanguageAsync(string language, Guid targetUid, CancellationToken cancellationToken = default)
    {
        var entities = await _notificationDbContext.NotificationTokens
            .Where(x => x.TargetUid == targetUid)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.Language = language;
            _logger.LogInformation($"Notification token language saved: Uid: {entity.Uid}; TargetUid: {targetUid}; Language: {language}");
        }

        await _notificationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(IEnumerable<NotificationToken> notificationToken, CancellationToken cancellationToken = default)
    {
        foreach (var token in notificationToken)
            await SaveAsync(token, cancellationToken);
    }

    public async Task RemoveAsync(Guid targetUid, NotificationTokenType type, CancellationToken cancellationToken = default)
    {
        var entities = await _notificationDbContext.NotificationTokens
            .Where(x => x.TargetUid == targetUid &&
                        x.Type == type)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            _logger.LogInformation($"Notification token removed: Uid: {entity.Uid}; TargetUid: {targetUid};");
        }

        await _notificationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Guid targetUid, NotificationTokenType type, string deviceUid, CancellationToken cancellationToken = default)
    {
        var entities = await _notificationDbContext.NotificationTokens
            .Where(x => x.TargetUid == targetUid &&
                        x.DeviceId == deviceUid &&
                        x.Type == type)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            _logger.LogInformation($"Notification token removed: Uid: {entity.Uid}; TargetUid: {targetUid};");
        }

        await _notificationDbContext.SaveChangesAsync(cancellationToken);
    }
}