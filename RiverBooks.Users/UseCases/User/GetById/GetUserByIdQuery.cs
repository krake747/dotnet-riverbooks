using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.User.GetById;

internal sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>;

internal sealed record GetUserByIdHandler(IApplicationUserRepository userRepository) 
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken token = default)
    {
        var user = await userRepository.GetUserByIdAsync(request.UserId, token);
        if (user is null)
        {
            return Result.NotFound();
        }

        return new UserDto(Guid.Parse(user.Id), user.Email!);
    }
}