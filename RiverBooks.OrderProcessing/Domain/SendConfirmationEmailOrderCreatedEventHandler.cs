using MediatR;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Domain;

internal sealed class SendConfirmationEmailOrderCreatedEventHandler(ISender mediator) : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent notification, CancellationToken token = default)
    {
        var userByIdQuery = new UserDetailsByIdQuery(notification.Order.UserId);

        var result = await mediator.Send(userByIdQuery, token);

        if (result.IsSuccess is false)
        {
            // TODO: Log the error
            return;
        }

        var userEmail = result.Value.EmailAddress;
        
        var command = new SendEmailCommand
        {
            To = userEmail,
            From = "noreply@test.com",
            Subject = "Your RiverBooks Purchase",
            Body = $"You bought {notification.Order.OrderItems.Count} items."
        };

        var emailId = await mediator.Send(command, token);
        
        // TODO: Do something with emailId
    }
}