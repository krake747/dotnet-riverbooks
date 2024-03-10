using FastEndpoints;

namespace RiverBooks.Books.BookEndpoints;

internal sealed class GetAll(IBookService bookService) 
    : EndpointWithoutRequest<GetAllBooksResponse>
{
    public override void Configure()
    {
        Get("/books");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(CancellationToken token)
    {
        await SendAsync(new GetAllBooksResponse
        {
            Books = await bookService.ListBooksAsync()
        }, cancellation: token);
    }
}

public sealed class GetAllBooksResponse
{
    public required IEnumerable<BookDto> Books { get; init; } = [];
}