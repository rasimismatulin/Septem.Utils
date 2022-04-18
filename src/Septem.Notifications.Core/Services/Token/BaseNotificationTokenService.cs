using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Entities;
using Septem.Notifications.Core.Infrastructure;

namespace Septem.Notifications.Core.Services.Token;

internal class BaseNotificationTokenService
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly INotificationTokenRepository NotificationTokenRepository;
    protected readonly ILogger Logger;
    protected readonly NotificationTokenType TokenType;
    public BaseNotificationTokenService(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, INotificationTokenRepository notificationTokenRepository, NotificationTokenType tokenType)
    {
        ServiceProvider = serviceProvider;
        NotificationTokenRepository = notificationTokenRepository;
        Logger = loggerFactory.CreateLogger(GetType());
        TokenType = tokenType;
    }

    public async Task<ICollection<NotificationTokenEntity>> FindTokenByType(NotificationReceiverEntity receiverEntity, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(receiverEntity.Parameters))
            return new List<NotificationTokenEntity>();

        if (!string.IsNullOrWhiteSpace(receiverEntity.Token))
            return new List<NotificationTokenEntity> { new() { Token = receiverEntity.Token, Type = TokenType } };

        if (receiverEntity.TargetUid.HasValue)
            return await GetByTargetUid(receiverEntity.TargetUid.Value, cancellationToken);

        return new List<NotificationTokenEntity>();
    }

    protected virtual async Task<ICollection<NotificationTokenEntity>> GetByTargetUid(Guid targetUid, CancellationToken cancellationToken)
    {
        var tokens = await NotificationTokenRepository.CollectionQuery
            .Where(x => x.Type == TokenType)
            .Where(x => targetUid == x.TargetUid)
            .ToListAsync(cancellationToken);

        if (!tokens.Any())
            Logger.LogWarning($"Can't find token for Target: {targetUid}");

        return tokens;
    }
}