using FastEndpoints;

namespace RiverBooks.Books.BookEndpoints;

internal sealed class UpdateBookPrice(IBookService bookService) 
    : Endpoint<UpdateBookPriceRequest, BookDto?>
{
    public override void Configure()
    {
        Post("/books/{id}/price-history");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(UpdateBookPriceRequest request, CancellationToken token)
    {
        // TODO: Handle not found
        await bookService.UpdateBookPriceAsync(request.Id, request.NewPrice);

        var updatedBook = await bookService.GetBookByIdAsync(request.Id);
        if (updatedBook is null)
        {
            await SendNotFoundAsync(token);
        }
        
        await SendAsync(updatedBook, cancellation: token);
    }
}

public sealed class UpdateBookPriceRequest
{
    public required Guid Id { get; init; }
    public required decimal NewPrice { get; init; }
}