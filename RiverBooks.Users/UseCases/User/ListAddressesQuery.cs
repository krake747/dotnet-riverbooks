using Ardalis.Result;
using MediatR;
using RiverBooks.Users.UserEndpoints;

namespace RiverBooks.Users.UseCases.User;

internal sealed record ListAddressesQuery(string EmailAddress) 
    : IRequest<Result<List<UserAddressDto>>>;