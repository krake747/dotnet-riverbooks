using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.Cart.Checkout;

public sealed record CheckoutCartCommand(string EmailAddress, Guid shippingAddressId, Guid billingAddressId)
    : IRequest<Result<Guid>>;