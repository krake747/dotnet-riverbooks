using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.Cart.ListItems;

internal sealed class ListCartItemsHandler(IApplicationUserRepository userRepository)
    : IRequestHandler<ListCartItemsQuery, Result<IEnumerable<CartItemDto>>>
{
    public async Task<Result<IEnumerable<CartItemDto>>> Handle(ListCartItemsQuery request,
        CancellationToken token = default)
    {
        var user = await userRepository.GetUserWithCartByEmailAsync(request.EmailAddress, token);
        if (user is null)
        {
            return Result.Unauthorized();
        }

        return user.CartItems.Select(item =>
                new CartItemDto(item.Id, item.BookId, item.Description, item.Quantity, item.UnitPrice))
            .ToList();
    }
}