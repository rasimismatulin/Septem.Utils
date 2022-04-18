using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Infrastructure.Repositories;

internal class NotificationMessageRepository : INotificationMessageRepository
{
    private readonly NotificationDbContext _notificationDbContext;

    public NotificationMessageRepository(NotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
    }

    public IQueryable<NotificationMessageEntity> CollectionQuery => _notificationDbContext.NotificationMessages.Where(x => !x.IsDeleted);


    public async Task AddMessageAsync(NotificationMessageEntity message, CancellationToken cancellationToken)
    {
        await _notificationDbContext.NotificationMessages.AddAsync(message, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _notificationDbContext.SaveChangesAsync(cancellationToken);
    }
}