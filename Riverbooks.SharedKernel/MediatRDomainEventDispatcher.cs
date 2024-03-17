using MediatR;

namespace Riverbooks.SharedKernel;

public sealed class MediatRDomainEventDispatcher(IPublisher mediator) : IDomainEventDispatcher
{
    public async Task DispatchAndClearEvents(IEnumerable<IHaveDomainEvents> entitiesWithEvents)
    {
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            foreach (var domainEvent in events)
            {
                await mediator.Publish(domainEvent).ConfigureAwait(false);
            }
        }
    }
}