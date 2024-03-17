using MediatR;
using RiverBooks.Users.Contracts;
using Serilog;

namespace RiverBooks.Users.Integrations;

internal sealed class UserAddressIntegrationEventDispatcherHandler(ILogger logger, IPublisher mediator) 
    : INotificationHandler<AddressAddedEvent>
{
    public async Task Handle(AddressAddedEvent notification, CancellationToken token = default)
    {
        var userId = Guid.Parse(notification.NewAddress.UserId);

        var newStreetAddress = notification.NewAddress.StreetAddress;
        var addressDetails = new UserAddressDetails(userId,
            notification.NewAddress.Id,
            newStreetAddress.Street1,
            newStreetAddress.Street2,
            newStreetAddress.City,
            newStreetAddress.State,
            newStreetAddress.PostalCode,
            newStreetAddress.Country);

        await mediator.Publish(new NewUserAddressAddedIntegrationEvent(addressDetails), token);
        
        logger.Information("[DE Handler]New address integration event sent for {User}: {Address}",
            notification.NewAddress.UserId,
            newStreetAddress);
    }
}