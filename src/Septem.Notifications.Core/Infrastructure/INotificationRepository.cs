using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Infrastructure;

internal interface INotificationRepository
{
    IQueryable<NotificationEntity> CollectionQuery { get; }

    Task AddNotificationAsync(NotificationEntity notification, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}