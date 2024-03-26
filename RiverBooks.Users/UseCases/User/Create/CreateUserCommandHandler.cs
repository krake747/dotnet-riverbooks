using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.UseCases.User.Create;

internal sealed class CreateUserCommandHandler(UserManager<ApplicationUser> userManager, ISender mediator) 
    : IRequestHandler<CreateUserCommand, Result>
{
    public async Task<Result> Handle(CreateUserCommand command, CancellationToken token = default)
    {
        var newUser = new ApplicationUser
        {
            Email = command.Email,
            UserName = command.Email
        };

        var result = await userManager.CreateAsync(newUser, command.Password);
        if (result.Succeeded is false)
        {
            return Result.Error(result.Errors.Select(e => e.Description).ToArray());
        }

        // send welcome email
        var sendEmailCommand = new SendEmailCommand
        {
            To = command.Email,
            From = "donotreply@test.com",
            Subject = "Welcome to RiverBooks",
            Body = "Thank you for registering"
        };

        _ = await mediator.Send(sendEmailCommand, token);
        
        return Result.Error(result.Errors.Select(e => e.Description).ToArray());
    } 
}