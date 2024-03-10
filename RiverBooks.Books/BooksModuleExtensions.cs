using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Books.Data;

namespace RiverBooks.Books;

public static class BooksModuleExtensions
{
    public static IServiceCollection AddBooksModule(this IServiceCollection services, ConfigurationManager config)
    {
        var connectionString = config.GetConnectionString("Books");

        services.AddDbContext<BooksDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IBookRepository, EfBookRepository>();
        
        services.AddScoped<IBookService, BookService>();

        return services;
    }
}