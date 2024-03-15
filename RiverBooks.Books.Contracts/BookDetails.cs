using Ardalis.Result;
using MediatR;

namespace RiverBooks.Books.Contracts;

public sealed record BookDetailsResponse(Guid BookId, string Title, string Author, decimal Price);

public sealed record BookDetailsQuery(Guid BookId) : IRequest<Result<BookDetailsResponse>>;

