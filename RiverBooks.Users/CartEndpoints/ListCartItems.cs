using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.UseCases.Cart.ListItems;

namespace RiverBooks.Users.CartEndpoints;

public sealed class CartResponse
{
    public IEnumerable<CartItemDto> CartItems { get; set; } = [];
}

internal sealed class ListCartItems(ISender mediator) : EndpointWithoutRequest<CartResponse>
{
    public override void Configure()
    {
        Get("/cart");
        Claims("EmailAddress");
    }

    public override async Task HandleAsync(CancellationToken token)
    {
        var emailAddress = User.FindFirstValue("EmailAddress");

        var query = new ListCartItemsQuery(emailAddress!);

        var result = await mediator.Send(query, token);

        if (result.Status is ResultStatus.Unauthorized)
        {
            await SendUnauthorizedAsync(token);
        }
        else
        {
            await SendOkAsync(new CartResponse
            {
                CartItems = result.Value
            }, token);
        }
    }
}