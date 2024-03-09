using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace RiverBooks.Books;

internal interface IBookService
{
    IEnumerable<BookDto> ListBooks();
}

internal sealed class BookService : IBookService
{
    public IEnumerable<BookDto> ListBooks() =>
    [
        new BookDto(Guid.NewGuid(), "The Fellowship of the Ring", "J.R.R. Tolkien"),
        new BookDto(Guid.NewGuid(), "The Two Towers", "J.R.R. Tolkien"),
        new BookDto(Guid.NewGuid(), "The Return of the Ring", "J.R.R. Tolkien")
    ];
}

public sealed record BookDto(Guid Id, string Title, string Author);

public static class BookEndpoints
{
    public const string GetAll = "/books";

    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(GetAll, (IBookService bookService) => bookService.ListBooks());

        return app;
    }
}

public sealed class ListBooksResponse
{
    public IEnumerable<BookDto> Books { get; set; }
}

internal class ListBooksEndpoint(IBookService bookService) 
    : EndpointWithoutRequest<ListBooksResponse>
{
    public override void Configure()
    {
        Get("/api/books");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(CancellationToken token)
    {
        await SendAsync(new ListBooksResponse
        {
            Books = bookService.ListBooks()
        }, cancellation: token);
    }
}

public static class BookServiceCollectionExtensions
{
    public static IServiceCollection AddBookServices(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();

        return services;
    }
}