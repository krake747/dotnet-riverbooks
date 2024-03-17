using Ardalis.Result;
using MediatR;
using RiverBooks.Users.UserEndpoints;

namespace RiverBooks.Users.UseCases.User;

internal class ListAddressesQueryHandler(IApplicationUserRepository userRepository)
    : IRequestHandler<ListAddressesQuery, Result<List<UserAddressDto>>>
{
    public async Task<Result<List<UserAddressDto>>> Handle(ListAddressesQuery request,
        CancellationToken token = default)
    {
        var user = await userRepository.GetUserWithAddressesByEmailAsync(request.EmailAddress, token);
        if (user is null)
        {
            return Result.Unauthorized();
        }

        return user.Addresses
            .Select(ua => new UserAddressDto(ua.Id, ua.StreetAddress.Street1,
                ua.StreetAddress.Street2,
                ua.StreetAddress.City,
                ua.StreetAddress.State,
                ua.StreetAddress.PostalCode,
                ua.StreetAddress.Country))
            .ToList();
    }
}