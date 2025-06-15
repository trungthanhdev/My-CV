using MediatR;

namespace ZEN.Common.Domain.Events;
public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent
{
}