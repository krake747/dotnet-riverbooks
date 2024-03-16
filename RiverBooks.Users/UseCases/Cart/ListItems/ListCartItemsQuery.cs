using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.Cart.ListItems;

public sealed record ListCartItemsQuery(string EmailAddress) : IRequest<Result<IEnumerable<CartItemDto>>>;