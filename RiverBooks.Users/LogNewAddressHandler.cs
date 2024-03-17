using MediatR;
using RiverBooks.Users.Domain;
using Serilog;

namespace RiverBooks.Users;

internal sealed class LogNewAddressHandler(ILogger logger) : INotificationHandler<AddressAddedEvent>
{
    public Task Handle(AddressAddedEvent notification, CancellationToken token = default)
    {
        logger.Information("[DE Handler]New address added to user {User}: {Address}",
            notification.NewAddress.UserId,
            notification.NewAddress.StreetAddress);

        return Task.CompletedTask;
    }
}