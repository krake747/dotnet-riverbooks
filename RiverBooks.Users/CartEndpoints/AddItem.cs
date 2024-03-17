using System.Security.Claims;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.UseCases.Cart.AddItem;

namespace RiverBooks.Users.CartEndpoints;

public sealed record AddCartItemRequest(Guid BookId, int Quantity);

internal class AddItem(ISender mediator) : Endpoint<AddCartItemRequest>
{
    public override void Configure()
    {
        Post("/cart");
        Claims("EmailAddress");
    }

    public override async Task HandleAsync(AddCartItemRequest request, CancellationToken token)
    {
        var emailAddress = User.FindFirstValue("EmailAddress");

        var command = new AddItemToCartCommand(request.BookId, request.Quantity, emailAddress!);

        var result = await mediator.Send(command, token);

        switch (result.Status)
        {
            case ResultStatus.Unauthorized:
                await SendUnauthorizedAsync(token);
                break;
            case ResultStatus.Invalid:
                await SendResultAsync(result.ToMinimalApiResult());
                break;
            default:
                await SendOkAsync(token);
                break;
        }
    }
}