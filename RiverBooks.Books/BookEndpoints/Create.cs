using FastEndpoints;

namespace RiverBooks.Books.BookEndpoints;

internal sealed class Create(IBookService bookService)
    : Endpoint<CreateBookRequest, BookDto>
{
    public override void Configure()
    {
        Post("/books");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateBookRequest request, CancellationToken token)
    {
        var newBookDto = new BookDto(request.Id ?? Guid.NewGuid(), request.Title, request.Author, request.Price);

        await bookService.CreateBookAsync(newBookDto);

        await SendCreatedAtAsync<GetBookById>(new { newBookDto.Id }, newBookDto, cancellation: token);
    }
}

public sealed class CreateBookRequest
{
    public required Guid? Id { get; init; }
    public required string Title { get; init; } = string.Empty;
    public required string Author { get; init; } = string.Empty;
    public required decimal Price { get; init; }
}