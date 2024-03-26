using FastEndpoints;
using MediatR;
using Ardalis.Result.AspNetCore;
using RiverBooks.Users.UseCases.User.Create;

namespace RiverBooks.Users.UserEndpoints;

public sealed record CreateUserRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

internal sealed class Create(ISender mediator) : Endpoint<CreateUserRequest>
{
    public override void Configure()
    {
        Post("/users");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateUserRequest request, CancellationToken token)
    {
        var command = new CreateUserCommand(request.Email, request.Password);
        
        var result = await mediator.Send(command, token);

        if (result.IsSuccess is false)
        {
            await SendResultAsync(result.ToMinimalApiResult());
            return;
        }
        
        await SendOkAsync(token);
    }
}