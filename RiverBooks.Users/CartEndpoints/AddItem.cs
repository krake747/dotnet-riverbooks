using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;

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

        if (result.Status is ResultStatus.Unauthorized)
        {
            await SendUnauthorizedAsync(token);
        }
        else
        {
            await SendOkAsync(token);
        }
    }
}