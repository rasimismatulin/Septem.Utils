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

internal class FcmOrSmsNotificationTokenService : BaseNotificationTokenService, INotificationTokenFindService
{
    public FcmOrSmsNotificationTokenService(NotificationDbContext notificationDbContext, IServiceProvider serviceProvider, ILoggerFactory loggerFactory) :
        base(serviceProvider, loggerFactory, notificationDbContext, NotificationTokenType.Fcm)
    {
    }

    public async Task<ICollection<NotificationTokenEntity>> GetTokensAsync(NotificationReceiverEntity receiverEntity, CancellationToken cancellationToken)
    {
        return await FindTokenByType(receiverEntity, cancellationToken);
    }

    protected override async Task<ICollection<NotificationTokenEntity>> GetByTargetUid(Guid targetUid, CancellationToken cancellationToken)
    {
        var tokens = await NotificationDbContext.NotificationTokens
            .Where(x => x.Type == TokenType && targetUid == x.TargetUid)
            .ToListAsync(cancellationToken);

        if (!tokens.Any())
        {
            var newTokens = await NotificationDbContext.NotificationTokens
                .Where(x => x.Type == NotificationTokenType.Sms && targetUid == x.TargetUid)
                .ToListAsync(cancellationToken);

            tokens.AddRange(newTokens);
        }

        if (!tokens.Any())
            Logger.LogWarning($"Can't find token for Target: {targetUid}");


        return tokens;
    }
}