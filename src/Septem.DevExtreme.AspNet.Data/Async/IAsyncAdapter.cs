using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Septem.DevExtreme.AspNet.Data.Async {

    public interface IAsyncAdapter {
        Task<int> CountAsync(IQueryProvider queryProvider, Expression expr, CancellationToken cancellationToken);
        Task<IEnumerable<T>> ToEnumerableAsync<T>(IQueryProvider queryProvider, Expression expr, CancellationToken cancellationToken);
    }

}
