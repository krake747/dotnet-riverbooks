using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;

namespace RiverBooks.Users.UsersEndpoints;

public sealed class UserLoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

internal sealed class Login(UserManager<ApplicationUser> userManager) : Endpoint<UserLoginRequest>
{
    public override void Configure()
    {
        Post("users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UserLoginRequest request, CancellationToken token)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            await SendUnauthorizedAsync(token);
            return;
        }

        var loginSuccessful = await userManager.CheckPasswordAsync(user, request.Password);
        if (loginSuccessful is false)
        {
            await SendUnauthorizedAsync(token);
            return;
        }

        var jwtSecret = Config["Auth:JwtSecret"]!;
        var jwtToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = jwtSecret;
            o.User["EmailAddress"] = request.Email;
        });

        await SendOkAsync(jwtToken, token);
    }
}