using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Septem.Utils.Helpers.ActionInvoke.Collection;

namespace Septem.Utils.Helpers.ActionInvoke;

public class Result
{
    public Result(Guid requestId)
    {
        if (requestId == default)
            throw new ArgumentException("Request argument is default", nameof(requestId));
        RequestId = requestId;
    }

    public Result(Guid requestId, Issue issue)
        : this(requestId)
    {
        Issue = issue ?? throw new ArgumentException("Issue argument is null", nameof(issue));
    }

    public Guid RequestId { get; }

    public Issue Issue { get; }

    public bool IsFailure => Issue != null;

    public bool IsSuccess => Issue == null;
}

public class Result<T> : Result
{
    public Result(Guid requestId)
        : base(requestId)
    {
        Payload = default;
    }

    public Result(Guid requestId, T payload)
        : base(requestId)
    {
        Payload = payload;
    }

    public Result(Guid requestId, Issue issue)
        : base(requestId, issue)
    {
    }

    public T Payload { get; }
}

public class ResultOfCollection<T> : Result, IEnumerable<T>
{
    public ResultOfCollection(Guid requestId, IEnumerable<T> collection)
        : base(requestId)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));
        Collection = collection.ToArray();
    }

    public ResultOfCollection(Guid requestId, Issue issue)
        : base(requestId, issue)
    {
    }

    public IEnumerable<T> Collection { get; private set; } = Enumerable.Empty<T>();

    public IEnumerator<T> GetEnumerator() => Collection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Collection.GetEnumerator();

    public PaginationResult PaginationResultData { get; set; }

    public void FilterCollection(Func<T, bool> predicate)
    {
        Collection = Collection.Where(predicate);
    }
}
