using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.UseCases.User;

namespace RiverBooks.Users.UserEndpoints;

internal sealed record AddAddressRequest(
    string Street1,
    string Street2,
    string City,
    string State,
    string PostalCode,
    string Country);

internal sealed class AddAddress(ISender mediator) : Endpoint<AddAddressRequest>
{
    public override void Configure()
    {
        Post("/users/addresses");
        Claims("EmailAddress");
    }

    public override async Task HandleAsync(AddAddressRequest request,
        CancellationToken token)
    {
        var emailAddress = User.FindFirstValue("EmailAddress");

        var command = new AddAddressToUserCommand(emailAddress!,
            request.Street1,
            request.Street2,
            request.City,
            request.State,
            request.PostalCode,
            request.Country);

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