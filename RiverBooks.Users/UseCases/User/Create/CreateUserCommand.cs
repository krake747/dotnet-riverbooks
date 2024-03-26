using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.User.Create;

internal sealed record CreateUserCommand(string Email, string Password) : IRequest<Result>;