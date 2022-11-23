
using System;
using System.Threading.Tasks;

namespace Septem.Utils.Domain.Services;

public interface IAccessService<in TDomain>
{
    Task<bool> CanCreate(TDomain domain, Guid executorUid);
    Task<bool> CanEdit(TDomain domain, Guid executorUid);
    Task<bool> CanDelete(TDomain domain, Guid executorUid);
}
