namespace RiverBooks.OrderProcessing.Domain;

public sealed class Order
{
    private readonly List<OrderItem> _orderItems = [];

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public Address ShippingAddress { get; private set; } = default!;
    public Address BillingAddress { get; private set; } = default!;
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

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

        order.AddOrderItems(orderItems);
        return order;
    }
}