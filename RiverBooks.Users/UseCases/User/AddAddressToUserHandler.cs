using Ardalis.Result;
using MediatR;
using Serilog;

namespace RiverBooks.Users.UseCases.User;

internal class AddAddressToUserHandler(IApplicationUserRepository userRepository, ILogger logger)
    : IRequestHandler<AddAddressToUserCommand, Result>
{
    public async Task<Result> Handle(AddAddressToUserCommand request, CancellationToken token = default)
    {
        var user = await userRepository.GetUserWithAddressesByEmailAsync(request.EmailAddress, token);

        if (user is null)
        {
            return Result.Unauthorized();
        }

        var addressToAdd = new Address(request.Street1,
            request.Street2,
            request.City,
            request.State,
            request.PostalCode,
            request.Country);

        var userAddress = user.AddAddress(addressToAdd);

        await userRepository.SaveChangesAsync(token);

        logger.Information("[UseCase] Added address {Address} to user {Email} (Total: {Total})",
            userAddress.StreetAddress,
            request.EmailAddress,
            user.Addresses.Count);

        return Result.Success();
    }
}