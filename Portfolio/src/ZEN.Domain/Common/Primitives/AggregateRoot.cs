using System.Text.Json.Serialization;
using CTCore.DynamicQuery.Core.Domain;
using ZEN.Common.Domain.Events;

namespace ZEN.Domain.Common.Primitives;

public abstract class AggregateRoot : CTBaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(string id) : base(id)
    {
    }

    protected AggregateRoot()
    {
    }

    [JsonIgnore]
    public ICollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}