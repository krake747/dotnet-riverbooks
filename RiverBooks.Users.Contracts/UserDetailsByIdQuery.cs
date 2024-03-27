using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.Contracts;

public sealed record UserDetailsByIdQuery(Guid userId) 
    : IRequest<Result<UserDetails>>;