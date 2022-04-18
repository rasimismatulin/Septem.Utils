using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Infrastructure;

internal interface INotificationMessageRepository
{
    IQueryable<NotificationMessageEntity> CollectionQuery { get; }

    Task AddMessageAsync(NotificationMessageEntity message, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}