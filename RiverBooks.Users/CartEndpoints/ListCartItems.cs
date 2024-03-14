using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;

namespace RiverBooks.Users.CartEndpoints;

internal sealed class ListCartItems(ISender mediator) : EndpointWithoutRequest<CartResponse>
{
    public override void Configure()
    {
        Get("/cart");
        Claims("EmailAddress");
    }

    public override async Task HandleAsync(CancellationToken token)
    {
        var emailAddress = User.FindFirstValue("EmailAddress");

        var query = new ListCartItemsQuery(emailAddress!);

        var result = await mediator.Send(query, token);

        if (result.Status is ResultStatus.Unauthorized)
        {
            await SendUnauthorizedAsync(token);
        }
        else
        {
            await SendOkAsync(new CartResponse
            {
                CartItems = result.Value
            }, token);
        }
    }
}

public sealed class CartResponse
{
    public IEnumerable<CartItemDto> CartItems { get; set; } = [];
}

public sealed record CartItemDto(Guid Id, Guid BookId, string Description, int Quantity, decimal UnitPrice);

public sealed record ListCartItemsQuery(string EmailAddress) : IRequest<Result<IEnumerable<CartItemDto>>>;

internal sealed class ListCartItemsQueryHandler(IApplicationUserRepository userRepository)
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