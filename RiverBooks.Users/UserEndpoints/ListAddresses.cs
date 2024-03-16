using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.UseCases.User;

namespace RiverBooks.Users.UserEndpoints;

public record UserAddressDto(
    Guid Id,
    string Street1,
    string Street2,
    string City,
    string State,
    string PostalCode,
    string Country);

public class AddressListResponse
{
    public required IEnumerable<UserAddressDto> Addresses { get; init; } = [];
}

internal class ListAddresses(ISender mediator) :
    EndpointWithoutRequest<AddressListResponse>
{
    public override void Configure()
    {
        Get("/users/addresses");
        Claims("EmailAddress");
    }

    public override async Task HandleAsync(CancellationToken token)
    {
        var emailAddress = User.FindFirstValue("EmailAddress");

        var query = new ListAddressesQuery(emailAddress!);

        var result = await mediator.Send(query, token);

        if (result.Status == ResultStatus.Unauthorized)
        {
            await SendUnauthorizedAsync(token);
        }
        else
        {
            var response = new AddressListResponse
            {
                Addresses = result.Value
            };

            await SendAsync(response, cancellation: token);
        }
    }
}