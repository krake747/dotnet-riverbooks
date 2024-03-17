using System.Text.Json;
using Ardalis.GuardClauses;
using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Contracts;
using Serilog;
using StackExchange.Redis;

namespace RiverBooks.OrderProcessing;

public sealed record Address(
    string Street1,
    string Street2,
    string City,
    string State,
    string PostalCode,
    string Country);

internal sealed record OrderAddress(Guid Id, Address Address);

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

public interface IOrderRepository
{
    Task<List<Order>> ListAsync(CancellationToken token = default);
    Task AddAsync(Order order, CancellationToken token = default);
    Task SaveChangesAsync(CancellationToken token = default);
}

internal interface IOrderAddressCache
{
    Task<Result<OrderAddress>> GetByIdAsync(Guid addressId);
    Task<Result> StoreAsync(OrderAddress orderAddress);
}

internal sealed class RedisOrderAddressCache: IOrderAddressCache
{
    private readonly IDatabase _db;
    private readonly ILogger _logger;

    public RedisOrderAddressCache(ILogger logger)
    {
        var redis = ConnectionMultiplexer.Connect("localhost"); // TODO: Get server from config
        _db = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<Result<OrderAddress>> GetByIdAsync(Guid id)
    {
        string? fetchedJson = await _db.StringGetAsync(id.ToString());
        if (fetchedJson is null)
        {
            _logger.Warning("Address {Id} not found in {Db}", id, "REDIS");
            return Result.NotFound();
        }
        
        var address = JsonSerializer.Deserialize<OrderAddress>(fetchedJson);
        if (address is null)
        {
            return Result.NotFound();
        }

        _logger.Information("Address {Id} returned from {Db}", id, "REDIS");
        return Result.Success(address);
    }

    public async Task<Result> StoreAsync(OrderAddress orderAddress)
    {
        var key = orderAddress.Id.ToString();
        var addressJson = JsonSerializer.Serialize(orderAddress);

        await _db.StringSetAsync(key, addressJson);
        _logger.Information("Address {Id} stored in {Db}", orderAddress.Id, "REDIS");

        return Result.Success();
    }
}

internal sealed class ReadThroughOrderAddressCache(ILogger logger, ISender mediator, RedisOrderAddressCache redisCache) : IOrderAddressCache
{
    public async Task<Result<OrderAddress>> GetByIdAsync(Guid addressId)
    {
        var result = await redisCache.GetByIdAsync(addressId);
        if (result.IsSuccess)
        {
            return result;
        }

        if (result.Status is not ResultStatus.NotFound)
        {
            return Result.NotFound();
        }

        logger.Information("Address {Id} not found; fetching from source", addressId);
        var query = new UserAddressDetailsByIdQuery(addressId);

        var queryResult = await mediator.Send(query);
        if (queryResult.IsSuccess is false)
        {
            return Result.NotFound();
        }

        var dto = queryResult.Value;
        var address = new Address(dto.Street1,
            dto.Street2,
            dto.City,
            dto.State,
            dto.PostalCode,
            dto.Country);
        
        var orderAddress = new OrderAddress(dto.AddressId, address);
        await StoreAsync(orderAddress);
        return orderAddress;
    }

    public Task<Result> StoreAsync(OrderAddress orderAddress) => redisCache.StoreAsync(orderAddress);
}
