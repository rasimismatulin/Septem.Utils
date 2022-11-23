using System;
using System.Collections.Generic;

namespace Septem.Utils.Helpers.Extensions;

public sealed class ChangeResult<TLocal, TRemote>
{
    public ChangeResult(IEnumerable<TLocal> deleted, IEnumerable<Tuple<TLocal, TRemote>> changed, IEnumerable<TRemote> inserted)
    {
        Deleted = deleted;
        Changed = changed;
        Inserted = inserted;
    }

    public IEnumerable<TLocal> Deleted { get; }

    public IEnumerable<Tuple<TLocal, TRemote>> Changed { get; }

    public IEnumerable<TRemote> Inserted { get; }
}
