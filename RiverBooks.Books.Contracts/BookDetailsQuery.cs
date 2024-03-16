using Ardalis.Result;
using MediatR;

namespace RiverBooks.Books.Contracts;

public sealed record BookDetailsQuery(Guid BookId) : IRequest<Result<BookDetailsResponse>>;