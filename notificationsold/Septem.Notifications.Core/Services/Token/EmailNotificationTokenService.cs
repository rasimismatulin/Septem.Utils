using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Entities;
using Septem.Notifications.Core.Infrastructure;

namespace Septem.Notifications.Core.Services.Token;

internal class EmailNotificationTokenService : BaseNotificationTokenService, INotificationTokenFindService
{
    public EmailNotificationTokenService(NotificationDbContext notificationDbContext, IServiceProvider serviceProvider, ILogger<EmailNotificationTokenService> logger) :
        base(serviceProvider, logger, notificationDbContext, NotificationTokenType.Email)
    {

    }

    public async Task<ICollection<NotificationTokenEntity>> GetTokensAsync(NotificationReceiverEntity receiverEntity, CancellationToken cancellationToken)
    {
        return await FindTokenByType(receiverEntity, cancellationToken);
    }
}