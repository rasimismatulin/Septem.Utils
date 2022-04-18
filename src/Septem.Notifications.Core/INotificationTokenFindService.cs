using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core;

internal interface INotificationTokenFindService
{
    Task<ICollection<NotificationTokenEntity>> GetTokensAsync(NotificationReceiverEntity receiverEntity, CancellationToken cancellationToken);
}