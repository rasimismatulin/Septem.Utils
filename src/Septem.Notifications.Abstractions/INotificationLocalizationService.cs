using System;
using System.Threading;
using System.Threading.Tasks;

namespace Septem.Notifications.Abstractions;

public interface INotificationLocalizationService : IDisposable, IAsyncDisposable
{
    Task LocalizeNotificationAsync(Notification notification, CancellationToken cancellationToken);
}