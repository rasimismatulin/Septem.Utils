using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Septem.Utils.Domain;

public abstract class BaseDomain
{
    private readonly List<DomainEventBase> _domainEvents = new();

    public Guid Id { get; set; }

    [NotMapped]
    public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);

    internal void ClearDomainEvents() => _domainEvents.Clear();
}