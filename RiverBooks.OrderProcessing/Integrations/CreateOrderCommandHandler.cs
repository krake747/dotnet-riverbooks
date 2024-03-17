using Ardalis.Result;
using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using Serilog;

namespace RiverBooks.OrderProcessing.Integrations;

internal sealed class CreateOrderCommandHandler(
    ILogger logger,
    IOrderRepository orderRepository,
    IOrderAddressCache addressCache)
    : IRequestHandler<CreateOrderCommand, Result<OrderDetailsResponse>>
{
    public async Task<Result<OrderDetailsResponse>> Handle(CreateOrderCommand request,
        CancellationToken token = default)
    {
        var items = request.OrderItems.Select(x => new OrderItem(x.BookId, x.Description, x.Quantity, x.UnitPrice));

        var shippingAddress = await addressCache.GetByIdAsync(request.ShippingAddressId);
        var billingAddress = await addressCache.GetByIdAsync(request.BillingAddressId);

        var newOrder = Order.Create(request.UserId,
            shippingAddress.Value.Address,
            billingAddress.Value.Address,
            items);

        await orderRepository.AddAsync(newOrder, token);
        await orderRepository.SaveChangesAsync(token);

        logger.Information("New Order Created! {OrderId}", newOrder.Id);

        return new OrderDetailsResponse(newOrder.Id);
    }
}