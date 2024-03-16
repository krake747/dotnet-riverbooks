using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.UseCases.Cart.Checkout;

namespace RiverBooks.Users.CartEndpoints;

public sealed record CheckoutRequest(Guid ShippingAddressId, Guid BillingAddressId);

public sealed record CheckoutResponse(Guid NewOrderId);

internal sealed class Checkout(ISender mediator) : Endpoint<CheckoutRequest, CheckoutResponse>
{
    public override void Configure()
    {
        Post("/cart/checkout");
        Claims("EmailAddress");
    }

    public override async Task HandleAsync(CheckoutRequest request,
        CancellationToken token)
    {
        var emailAddress = User.FindFirstValue("EmailAddress");

        var command = new CheckoutCartCommand(emailAddress!, request.ShippingAddressId, request.BillingAddressId);

        var result = await mediator.Send(command, token);

        if (result.Status is ResultStatus.Unauthorized)
        {
            await SendUnauthorizedAsync(token);
        }
        else
        {
            await SendOkAsync(new CheckoutResponse(result.Value), token);
        }
    }
}