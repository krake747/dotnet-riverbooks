using Microsoft.EntityFrameworkCore;

namespace RiverBooks.Books.Data;

internal sealed class EfBookRepository(BooksDbContext dbContext) : IBookRepository
{
    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default) =>
        await dbContext.Books.FindAsync([id], token);

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