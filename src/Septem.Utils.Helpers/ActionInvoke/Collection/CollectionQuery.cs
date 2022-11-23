using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Septem.Utils.Helpers.ActionInvoke.Collection.Enums;

namespace Septem.Utils.Helpers.ActionInvoke.Collection;

public class CollectionQuery
{
    public string OrderBy { get; set; }

    public OrderDirection OrderDirection { get; set; }

    public string SearchString { get; set; }

    public SearchOptions Comparison { get; set; }

    public JoinOptions Couple { get; set; }

    public ICollection<string> QueryFields { get; set; }

    public int PageSize { get; set; }

    public int PageNumber { get; set; }

    public CollectionQuery()
    {
        
    }

    public CollectionQuery(CollectionQuery collectionQuery)
    {
        OrderBy = collectionQuery.OrderBy;
        OrderDirection = collectionQuery.OrderDirection;
        SearchString = collectionQuery.SearchString;
        Comparison = collectionQuery.Comparison;
        Couple = collectionQuery.Couple;
        QueryFields = collectionQuery.QueryFields;
        PageNumber = collectionQuery.PageNumber;
        PageSize = collectionQuery.PageSize;
    }
}