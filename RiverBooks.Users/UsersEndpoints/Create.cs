using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace RiverBooks.Users.UsersEndpoints;

public sealed record CreateUserRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

internal sealed class Create(UserManager<ApplicationUser> userManager) : Endpoint<CreateUserRequest>
{
    public override void Configure()
    {
        Post("/users");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateUserRequest request, CancellationToken token)
    {
        var newUser = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email
        };

        await userManager.CreateAsync(newUser, request.Password);

        await SendOkAsync(token);
    }
}