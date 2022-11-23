using Septem.Utils.Helpers.ActionInvoke.Collection;

namespace Septem.Utils.Helpers.ActionInvoke;

public interface IRequestOfCollection<TSearch>
    where TSearch : CollectionQuery
{
    TSearch CollectionQuery { get; set; }
}
