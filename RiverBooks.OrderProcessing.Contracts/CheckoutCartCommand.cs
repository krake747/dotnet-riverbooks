using Ardalis.Result;
using MediatR;

namespace RiverBooks.OrderProcessing.Contracts;

public sealed record CreateOrderCommand(
    Guid UserId,
    Guid ShippingAddressId,
    Guid BillingAddressId,
    IEnumerable<OrderItemDetails> OrderItems)
    : IRequest<Result<OrderDetailsResponse>>;