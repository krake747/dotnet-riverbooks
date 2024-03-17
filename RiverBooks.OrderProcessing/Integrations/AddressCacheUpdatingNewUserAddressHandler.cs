using MediatR;
using RiverBooks.Users.Contracts;
using Serilog;

namespace RiverBooks.OrderProcessing.Integrations;

internal sealed class AddressCacheUpdatingNewUserAddressHandler(ILogger logger, IOrderAddressCache addressCache) 
    : INotificationHandler<NewUserAddressAddedIntegrationEvent>
{
    public async Task Handle(NewUserAddressAddedIntegrationEvent notification, 
        CancellationToken ct)
    {
        var orderAddress = new OrderAddress(notification.Details.AddressId,
            new Address(notification.Details.Street1,
                notification.Details.Street2,
                notification.Details.City,
                notification.Details.State,
                notification.Details.PostalCode,
                notification.Details.Country));

        await addressCache.StoreAsync(orderAddress);

        logger.Information("Cache updated with new address {Address}", orderAddress);
    }
}