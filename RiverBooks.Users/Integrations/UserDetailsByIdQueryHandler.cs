using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.UseCases.User.GetById;

namespace RiverBooks.Users.Integrations;

internal sealed class UserDetailsByIdQueryHandler(ISender mediator)
    : IRequestHandler<UserDetailsByIdQuery, Result<UserDetails>>
{
    public async Task<Result<UserDetails>> Handle(UserDetailsByIdQuery request, CancellationToken token = default)
    {
        var query = new GetUserByIdQuery(request.userId);

        var result = await mediator.Send(query, token);

        return result.Map(u => new UserDetails(u.UserId, u.EmailAddress));
    }
}