using Ardalis.Result;
using MediatR;
using RiverBooks.OrderProcessing.Contracts;

namespace RiverBooks.Users.UseCases.Cart.Checkout;

internal sealed class CheckoutCartHandler(IApplicationUserRepository userRepository, ISender mediator)
    : IRequestHandler<CheckoutCartCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CheckoutCartCommand request, CancellationToken token = default)
    {
        var user = await userRepository.GetUserWithCartByEmailAsync(request.EmailAddress, token);

        if (user is null)
        {
            return Result.Unauthorized();
        }

        var items = user.CartItems.Select(item =>
                new OrderItemDetails(item.BookId,
                    item.Quantity,
                    item.UnitPrice,
                    item.Description))
            .ToList();

        var createOrderCommand = new CreateOrderCommand(Guid.Parse(user.Id),
            request.shippingAddressId,
            request.billingAddressId,
            items);

        // TODO: Consider replacing with a message-based approach for perf reasons
        var result = await mediator.Send(createOrderCommand, token); // synchronous

        if (result.IsSuccess is false)
        {
            // Change from a Result<OrderDetailsResponse> to Result<Guid>
            return result.Map(x => x.OrderId);
        }

        user.ClearCart();
        await userRepository.SaveChangesAsync(token);

        return Result.Success(result.Value.OrderId);
    }
}