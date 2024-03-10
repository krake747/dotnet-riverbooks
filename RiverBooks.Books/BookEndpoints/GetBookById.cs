using FastEndpoints;

namespace RiverBooks.Books.BookEndpoints;

internal sealed class GetBookById(IBookService bookService) 
    : Endpoint<GetBookByIdRequest, BookDto>
{
    public override void Configure()
    {
        Get("/books/{id}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetBookByIdRequest request, CancellationToken token)
    {
        var book = await bookService.GetBookByIdAsync(request.Id);
        if (book is null)
        {
            await SendNotFoundAsync(token);
            return;
        }
        
        await SendAsync(book, cancellation: token);
    }
}

public sealed class GetBookByIdRequest
{
    public required Guid Id { get; init; }
}