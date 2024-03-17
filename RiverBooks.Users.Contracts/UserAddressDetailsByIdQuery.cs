using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.Contracts;

public sealed record UserAddressDetailsByIdQuery(Guid AddressId)
    : IRequest<Result<UserAddressDetails>>;