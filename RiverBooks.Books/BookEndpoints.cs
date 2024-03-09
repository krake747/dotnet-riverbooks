using System.Reflection;
using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RiverBooks.Books;

public static class BookServiceCollectionExtensions
{
    public static IServiceCollection AddBookServices(this IServiceCollection services, ConfigurationManager config)
    {
        var connectionString = config.GetConnectionString("Books");

        services.AddDbContext<BookDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IBookRepository, EfBookRepository>();
        
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

internal sealed class EfBookRepository(BookDbContext dbContext) : IBookRepository
{
    public async Task<Book?> GetByIdAsync(Guid id) => 
        await dbContext.Books.FindAsync(id);

    public async Task<IEnumerable<Book>> ListAsync() =>
        await dbContext.Books.ToListAsync();

    public Task AddAsync(Book book)
    {
        _ = dbContext.Add(book);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Book book)     
    {
        _ = dbContext.Remove(book);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync() => 
        await dbContext.SaveChangesAsync();
}

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

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    internal static readonly Guid Book1Guid = new ("A89F6CD7-4693-457B-9009-02205DBBFE45");
    internal static readonly Guid Book2Guid = new ("E4FA19BF-6981-4E50-A542-7C9B26E9EC31");
    internal static readonly Guid Book3Guid = new ("17C61E41-3953-42CD-8F88-D3F698869B35");
    
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(p => p.Title)
            .HasMaxLength(DataSchemaConstants.DefaultNameLength)
            .IsRequired();

        builder.Property(p => p.Author)
            .HasMaxLength(DataSchemaConstants.DefaultNameLength)
            .IsRequired();

        builder.HasData(GetSampleBookData());
    }
    
    private static IEnumerable<Book> GetSampleBookData()
    {
        const string tolkien = "J.R.R. Tolkien";
        yield return new Book(Book1Guid, "The Fellowship of the Ring", tolkien, 10.99m);
        yield return new Book(Book2Guid, "The Two Towers", tolkien, 11.99m);
        yield return new Book(Book3Guid, "The Return of the Ring", tolkien, 12.99m);
    } 
}

internal sealed class BookDbContext(DbContextOptions<BookDbContext> options) : DbContext(options)
{
    internal DbSet<Book> Books { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Books");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
            .HavePrecision(18, 6);
    }
}

internal static class DataSchemaConstants
{
    public const int DefaultNameLength = 100;
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
