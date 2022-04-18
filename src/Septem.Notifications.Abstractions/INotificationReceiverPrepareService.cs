using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Septem.Notifications.Abstractions;

public interface INotificationReceiverPrepareService
{
    Task<ICollection<Receiver>> PrepareReceiverAsync(Receiver receiver, CancellationToken cancellationToken);
}