namespace RiverBooks.Books.Contracts;

public sealed record BookDetailsResponse(
    Guid BookId,
    string Title,
    string Author,
    decimal Price);