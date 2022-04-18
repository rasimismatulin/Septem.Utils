using System.Threading;
using System.Threading.Tasks;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Services.Sender;

internal interface INotificationSenderService
{
    Task<SentStatus> SendAsync(Notification notification, NotificationMessageEntity message, NotificationTokenEntity token, CancellationToken cancellationToken);
}