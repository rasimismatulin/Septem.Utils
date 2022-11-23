using System;
using System.Globalization;
using System.Text.Json.Serialization;
using Septem.Utils.Helpers.ActionInvoke.Collection;

namespace Septem.Utils.Helpers.ActionInvoke;

public abstract class Request : ILanguageAwareRequest, IExecutorAwareRequest
{
    protected Request()
    {
        Id = Guid.NewGuid();
        InitiatedUtc = DateTime.UtcNow;
    }

    internal Request(Guid initiatorId) : this()
    {
        if (initiatorId == default)
            throw new ArgumentException(nameof(initiatorId));
        InitiatorId = initiatorId;
    }

    public Guid Id { get; }

    public Guid? InitiatorId { get; internal set; }

    public DateTime InitiatedUtc { get; }

    public string Language { get; private set; }

    [JsonIgnore]
    public CultureInfo CultureInfo { get; private set; }

    public Guid ExecutorUid { get; private set; }

    public abstract RequestType RequestType { get; }

    string ILanguageAwareRequest.Language
    {
        set
        {
            Language = value;
            CultureInfo = new CultureInfo(Language);
        }
    }

     

    Guid IExecutorAwareRequest.ExecutorUid
    {
        set => ExecutorUid = value;
    }
}
    
public abstract class RequestOfCollection<TSearch> : Request, IRequestOfCollection<TSearch>
    where TSearch : CollectionQuery, new()
{
    public TSearch CollectionQuery { get; set; }

    protected RequestOfCollection()
    {
        CollectionQuery = new TSearch();
    }

    protected RequestOfCollection(TSearch query)
    {
        CollectionQuery = query;
    }
}

public abstract class RequestOfCollection : Request
{
    protected RequestOfCollection()
    {
    }
}