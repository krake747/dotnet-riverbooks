using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Books.Data;

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