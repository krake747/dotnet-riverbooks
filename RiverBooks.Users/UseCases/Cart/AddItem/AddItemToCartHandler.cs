using Ardalis.Result;
using MediatR;
using RiverBooks.Books.Contracts;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;
using Serilog;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

internal sealed class AddItemToCartHandler(ILogger logger, ISender mediator, IApplicationUserRepository userRepository)
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

        logger.ForContext<AddItemToCartHandler>()
            .Information("Added an item to the cart {Description}", newCartItem.Description);
        return Result.Success();
    }
}