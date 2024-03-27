using System.ComponentModel.DataAnnotations.Schema;
using Riverbooks.SharedKernel;

namespace RiverBooks.OrderProcessing.Domain;

public sealed class Order : IHaveDomainEvents
{
    private readonly List<OrderItem> _orderItems = [];
    private readonly List<DomainEventBase> _domainEvents = [];

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public Address ShippingAddress { get; private set; } = default!;
    public Address BillingAddress { get; private set; } = default!;
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    
    [NotMapped]
    public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();
    private void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
    void IHaveDomainEvents.ClearDomainEvents() => _domainEvents.Clear();

    public DateTimeOffset DateCreated { get; private set; } = DateTimeOffset.Now;

    private void AddOrderItem(OrderItem item) => _orderItems.Add(item);
    private void AddOrderItems(IEnumerable<OrderItem> items) => _orderItems.AddRange(items);


    public static Order Create(Guid userId, Address shippingAddress, Address billingAddress,
        IEnumerable<OrderItem> orderItems)
    {
        var order = new Order
        {
            UserId = userId,
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress
        };

        var createdEvent = new OrderCreatedEvent(order);
        order.RegisterDomainEvent(createdEvent);
        
        order.AddOrderItems(orderItems);
        return order;
    }
}