using System;
using System.Collections.Generic;
using System.Linq;
using Septem.Utils.Helpers.ActionInvoke.Collection.Enums;

namespace Septem.Utils.Helpers.ActionInvoke.Collection;

public class CollectionSearchPropertyOption<T> : SearchPropertyOption, ICollectionSearchPropertyOption
{
    public ICollection<T> Collection { get; set; }

    public CollectionSearchPropertyOption()
    {
    }

    public CollectionSearchPropertyOption(JoinOptions joinOption, SearchOptions searchOption)
    {
        JoinOption = joinOption;
        SearchOption = searchOption;
    }

    public void Set(ICollection<T> value)
    {
        Collection = value;
    }

    public void SetAnd(ICollection<T> value)
    {
        Collection = value;
        JoinOption = JoinOptions.And;
    }

    public override string GetStringValue()
    {
        throw new NotImplementedException();
    }

    public override bool CanApply
    {
        get
        {
            if (!Equals(Collection, null))
                return Collection.Any();
            return false;
        }
    }

    public override object GetValue()
    {
        return Collection;
    }

    public object CollectionObject
    {
        get
        {
            return Collection;
        }
    }
}