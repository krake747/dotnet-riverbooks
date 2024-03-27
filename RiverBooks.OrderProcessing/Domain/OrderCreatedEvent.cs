using Riverbooks.SharedKernel;

namespace RiverBooks.OrderProcessing.Domain;

internal sealed class OrderCreatedEvent(Order order) : DomainEventBase
{
    public Order Order { get; } = order;
}