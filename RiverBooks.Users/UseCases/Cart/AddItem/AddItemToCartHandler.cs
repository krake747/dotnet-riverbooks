using Ardalis.Result;
using MediatR;
using RiverBooks.Books.Contracts;
using RiverBooks.Users.CartEndpoints;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

internal sealed class AddItemToCartHandler(IApplicationUserRepository userRepository, ISender mediator)
    : IRequestHandler<AddItemToCartCommand, Result>
{
    public async Task<Result> Handle(AddItemToCartCommand request, CancellationToken token = default)
    {
        var user = await userRepository.GetUserWithCartByEmailAsync(request.EmailAddress, token);
        if (user is null)
        {
            return Result.Unauthorized();
        }

        var query = new BookDetailsQuery(request.BookId);

        var result = await mediator.Send(query, token);
        if (result.IsSuccess is false)
        {
            return Result.NotFound();
        }

        var bookDetails = result.Value;
        var description = $"{bookDetails.Title} by {bookDetails.Author}";

        var newCartItem = new CartItem(request.BookId, description, request.Quantity, bookDetails.Price);

        user.AddItemToCart(newCartItem);

        await userRepository.SaveChangesAsync(token);
        return Result.Success();
    }
}