using Ardalis.GuardClauses;

namespace RiverBooks.OrderProcessing;

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
}

public sealed class OrderItem
{
    public OrderItem(Guid bookId, string description, int quantity, decimal unitPrice)
    {
        BookId = Guard.Against.Default(bookId);
        Description = Guard.Against.NullOrEmpty(description);
        Quantity = Guard.Against.Negative(quantity);
        UnitPrice = Guard.Against.Negative(unitPrice);
    }

    private OrderItem()
    {
        // EF
    }
    
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
}

public sealed record Address(
    string Street1,
    string Street2,
    string City,
    string State,
    string PostalCode,
    string Country);

public interface IOrderRepository
{
    Task<List<Order>> ListAsync(CancellationToken token = default);
    Task AddAsync(Order order, CancellationToken token = default);
    Task SaveChangesAsync(CancellationToken token = default);
}