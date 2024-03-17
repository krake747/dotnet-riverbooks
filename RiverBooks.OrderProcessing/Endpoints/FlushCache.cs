using FastEndpoints;
using Serilog;
using StackExchange.Redis;

namespace RiverBooks.OrderProcessing.Endpoints;

/// <summary>
///     Used for testing only
/// </summary>
internal sealed class FlushCache : EndpointWithoutRequest
{
    private readonly IDatabase _db;
    private readonly ILogger _logger;

    public FlushCache(ILogger logger)
    {
        // TODO: use DI
        var redis = ConnectionMultiplexer.Connect("localhost"); // TODO: Get server from config
        _db = redis.GetDatabase();
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/flushcache");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken token)
    {
        var result = await _db.ExecuteAsync("FLUSHDB");
        _logger.Warning("FLUSHED CACHE FOR {Db}", "REDIS");
    }
}