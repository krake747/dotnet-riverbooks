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

public static class BookServiceCollectionExtensions
{
    public static IServiceCollection AddBookService(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();

        return services;
    }
}