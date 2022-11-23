using System.Linq;

namespace Septem.Utils.Helpers.Dynamic;

public interface IDynamicQueryable
{
    public IQueryable Query { get; set; }
    string OrderByType { get; set; }
    string OrderBy { get; set; }

    public void Dispose();
}
