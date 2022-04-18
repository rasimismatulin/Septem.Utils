using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Infrastructure.Repositories;

internal class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _notificationDbContext;

    public NotificationRepository(NotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
    }

    public async Task AddNotificationAsync(NotificationEntity notification, CancellationToken cancellationToken)
    {
        await _notificationDbContext.Notifications.AddAsync(notification, cancellationToken);
    }

    public IQueryable<NotificationEntity> CollectionQuery => _notificationDbContext.Notifications.Where(x => !x.IsDeleted);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _notificationDbContext.SaveChangesAsync(cancellationToken);
    }
}