using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Septem.Notifications.Abstractions;

public interface INotificationTokenService
{
    Task<ICollection<NotificationToken>> GetAsync(Guid targetUid, CancellationToken cancellationToken = default);

    Task<ICollection<NotificationToken>> GetAsync(Guid targetUid, NotificationTokenType type, CancellationToken cancellationToken = default);

    Task<NotificationToken> GetAsync(Guid targetUid, NotificationTokenType type, string deviceUid, CancellationToken cancellationToken = default);

    Task SaveAsync(NotificationToken notificationToken, CancellationToken cancellationToken = default);

    Task SaveLanguageAsync(string language, Guid targetUid, NotificationTokenType tokenType, string deviceId, CancellationToken cancellationToken = default);

    Task SaveLanguageAsync(string language, Guid targetUid, NotificationTokenType tokenType, CancellationToken cancellationToken = default);

    Task SaveLanguageAsync(string language, Guid targetUid, CancellationToken cancellationToken = default);

    Task SaveAsync(IEnumerable<NotificationToken> notificationToken, CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid targetUid, NotificationTokenType type, CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid targetUid, NotificationTokenType type, string deviceUid, CancellationToken cancellationToken = default);
}