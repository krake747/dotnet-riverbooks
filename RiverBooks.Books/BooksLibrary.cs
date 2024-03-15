using Ardalis.GuardClauses;

namespace RiverBooks.Books;

internal interface IBookService
{
    Task<IEnumerable<BookDto>> ListBooksAsync();
    Task<BookDto?> GetBookByIdAsync(Guid id, CancellationToken token = default);
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

    public async Task<BookDto?> GetBookByIdAsync(Guid id, CancellationToken token = default)
    {
        var book = await bookRepository.GetByIdAsync(id, token);

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
    Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<Book>> ListAsync();
}

internal sealed class Book(Guid id, string title, string author, decimal price)
{
    public Guid Id { get; private set; } = Guard.Against.Default(id);
    public string Title { get; private set;} = Guard.Against.NullOrEmpty(title);
    public string Author { get; private set; } = Guard.Against.NullOrEmpty(author);
    public decimal Price { get; private set; } = Guard.Against.Negative(price);

    internal decimal UpdatePrice(decimal newPrice)
    {
        Price = Guard.Against.Negative(newPrice);
        return newPrice;
    }
}