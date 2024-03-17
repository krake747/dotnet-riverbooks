using Ardalis.Result;
using MediatR;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.Users.Contracts;
using Serilog;

namespace RiverBooks.OrderProcessing.Infrastructure;

internal sealed class ReadThroughOrderAddressCache(ILogger logger, ISender mediator, RedisOrderAddressCache redisCache)
    : IOrderAddressCache
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

        logger.ForContext<ReadThroughOrderAddressCache>()
            .Information("Address {Id} not found; fetching from source", addressId);

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