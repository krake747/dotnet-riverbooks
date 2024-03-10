using FastEndpoints;

namespace RiverBooks.Books.BookEndpoints;

internal sealed class Delete(IBookService bookService) 
    : Endpoint<DeleteBookRequest>
{
    public override void Configure()
    {
        Delete("/books/{id}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(DeleteBookRequest request, CancellationToken token)
    {
        // TODO: Handle not found
        await bookService.DeleteBookAsync(request.Id);
        await SendNoContentAsync(cancellation: token);
    }
}

public sealed class DeleteBookRequest
{
    public required Guid Id { get; init; }
}