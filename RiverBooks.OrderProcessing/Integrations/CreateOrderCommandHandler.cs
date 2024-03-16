using Ardalis.Result;
using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using Serilog;

namespace RiverBooks.OrderProcessing.Integrations;

internal sealed class CreateOrderCommandHandler(ILogger logger, IOrderRepository orderRepository)
    : IRequestHandler<CreateOrderCommand, Result<OrderDetailsResponse>> 
{
    public async Task<Result<OrderDetailsResponse>> Handle(CreateOrderCommand request, CancellationToken token = default)
    {
        var items = request.OrderItems.Select(x => new OrderItem(x.BookId, x.Description, x.Quantity, x.UnitPrice));

        var shippingAddress = new Address("123 Main", "", "Kent", "OH", "44444", "USA");
        var billingAddress = shippingAddress;
        
        var newOrder = Order.Factory.Create(request.UserId, shippingAddress, billingAddress, items);

        await orderRepository.AddAsync(newOrder, token);
        await orderRepository.SaveChangesAsync(token);

        logger.Information("New Order Created! {OrderId}", newOrder.Id);
        
        return new OrderDetailsResponse(newOrder.Id);
    }
}