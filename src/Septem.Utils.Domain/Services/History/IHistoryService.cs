using System;
using System.Threading;
using System.Threading.Tasks;

namespace Septem.Utils.Domain.Services.History;

public interface IHistoryService
{
    Task CreateHistory(BaseDomain domain, Guid executorUid, HistoryType type, CancellationToken cancellationToken);
}