using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace RiverBooks.Books;

public static class BookServiceCollectionExtensions
{
    public static IServiceCollection AddBookServices(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();

        return services;
    }
}

public static class BookEndpoints
{
    public const string GetAll = "/books";

    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(GetAll, async (IBookService bookService) => await bookService.ListBooksAsync());

        return app;
    }
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
            Books = await bookService.ListBooksAsync()
        }, cancellation: token);
    }
}

public sealed class ListBooksResponse
{
    public IEnumerable<BookDto> Books { get; set; } = [];
}

internal interface IBookService
{
    Task<IEnumerable<BookDto>> ListBooksAsync();
    Task<BookDto> GetBookByIdAsync(Guid id);
    Task CreateBookAsync(BookDto newBook);
    Task DeleteBookAsync(Guid id);
    Task UpdateBookPriceAsync(Guid bookId, decimal newPrice);
}

internal sealed class BookService(IBookRepository bookRepository) : IBookService
{
    public async Task<IEnumerable<BookDto>> ListBooksAsync()
    {
        var books = (await bookRepository.ListAsync())
            .Select(book => new BookDto(book.Id, book.Title, book.Author, book.Price));

        // [
        //     new BookDto(Guid.NewGuid(), "The Fellowship of the Ring", "J.R.R. Tolkien"),
        //     new BookDto(Guid.NewGuid(), "The Two Towers", "J.R.R. Tolkien"),
        //     new BookDto(Guid.NewGuid(), "The Return of the Ring", "J.R.R. Tolkien")
        // ];
        
        return books;
    }
    public async Task CreateBookAsync(BookDto newBook)
    {
        var book = new Book(newBook.Id, newBook.Title, newBook.Author, newBook.Price);

        await bookRepository.AddAsync(book);
        await bookRepository.SaveChangesAsync();
    }

    public async Task<BookDto> GetBookByIdAsync(Guid id)
    {
        var book = await bookRepository.GetByIdAsync(id);
        
        // TODO: handle not found case

        return new BookDto(book!.Id, book.Title, book.Author, book.Price);
    }

    public async Task UpdateBookPriceAsync(Guid bookId, decimal newPrice)
    {
        // validate the price

        var book = await bookRepository.GetByIdAsync(bookId);
        
        // handle not found case

        book!.UpdatePrice(newPrice);
        await bookRepository.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var bookToDelete = await bookRepository.GetByIdAsync(id);

        if (bookToDelete is not null)
        {
            await bookRepository.DeleteAsync(bookToDelete);
            await bookRepository.SaveChangesAsync();
        }
    }
}

public sealed record BookDto(Guid Id, string Title, string Author, decimal Price);

internal interface IBookRepository : IReadOnlyBookRepository
{
    Task AddAsync(Book book);
    Task DeleteAsync(Book book);
    Task SaveChangesAsync();
}

internal interface IReadOnlyBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<IEnumerable<Book>> ListAsync();
}


internal sealed class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public decimal Price { get; private set; }

    internal Book(Guid id, string title, string author, decimal price)
    {
        Id = Guard.Against.Default(id);
        Title = Guard.Against.NullOrEmpty(title);
        Author = Guard.Against.NullOrEmpty(author);
        Price = Guard.Against.Negative(price);
    }

    internal decimal UpdatePrice(decimal newPrice)
    {
        Price = Guard.Against.Negative(newPrice);
        return newPrice;
    }
}
