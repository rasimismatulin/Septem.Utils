using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Entities;
using Septem.Notifications.Core.Infrastructure;

namespace Septem.Notifications.Core.Services.Token;

internal class FcmNotificationTokenService : BaseNotificationTokenService, INotificationTokenFindService
{
    public FcmNotificationTokenService(NotificationDbContext notificationDbContext, IServiceProvider serviceProvider, ILogger<FcmNotificationTokenService> logger) :
        base(serviceProvider, logger, notificationDbContext, NotificationTokenType.Fcm)
    {
    }

    public async Task<ICollection<NotificationTokenEntity>> GetTokensAsync(NotificationReceiverEntity receiverEntity, CancellationToken cancellationToken)
    {
        return await FindTokenByType(receiverEntity, cancellationToken);
    }
}