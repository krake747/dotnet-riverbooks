using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;

namespace RiverBooks.Users.CartEndpoints;

internal class AddItem(ISender mediator) : Endpoint<AddCartItemRequest>
{
    public override void Configure()
    {
        Post("/cart");
        Claims("EmailAddress");
    }

    public override async Task HandleAsync(AddCartItemRequest request, CancellationToken token)
    {
        var emailAddress = User.FindFirstValue("EmailAddress");

        var command = new AddItemToCartCommand(request.BookId, request.Quantity, emailAddress!);

        var result = await mediator.Send(command, token);

        if (result.Status is ResultStatus.Unauthorized)
        {
            await SendUnauthorizedAsync(token);
        }
        else
        {
            await SendOkAsync(token);
        }
    }
}

public sealed record AddCartItemRequest(Guid BookId, int Quantity);

public sealed record AddItemToCartCommand(Guid BookId, int Quantity, string EmailAddress)
    : IRequest<Result>;
    
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

        // TODO: get description and price from Books Module
        var newCartItem = new CartItem(request.BookId, "description", request.Quantity, 1.0m);
        
        user.AddItemToCart(newCartItem);
        
        await userRepository.SaveChangesAsync(token);
        return Result.Success();
    }
}

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetUserWithCartByEmailAsync(string email, CancellationToken token = default);
    Task SaveChangesAsync(CancellationToken token = default);
}