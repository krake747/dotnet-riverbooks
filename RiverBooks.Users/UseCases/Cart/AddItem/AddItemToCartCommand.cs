using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.CartEndpoints;

public sealed record AddItemToCartCommand(Guid BookId, int Quantity, string EmailAddress)
    : IRequest<Result>;