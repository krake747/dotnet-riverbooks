using System.Text.Json;
using Ardalis.Result;
using Serilog;
using StackExchange.Redis;

namespace RiverBooks.OrderProcessing.Infrastructure;

internal sealed class RedisOrderAddressCache : IOrderAddressCache
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