using System.Threading;
using System.Threading.Tasks;

namespace Septem.Notifications.Abstractions;

public interface INotificationLocalizationService
{
    Task LocalizeNotificationAsync(Notification notification, CancellationToken cancellationToken);
}