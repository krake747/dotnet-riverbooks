using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Books.Data;
using Serilog;

namespace RiverBooks.Books;

public static class BooksModuleExtensions
{
    public static IServiceCollection AddBooksModule(this IServiceCollection services, ConfigurationManager config,
        ILogger logger)
    {
        var connectionString = config.GetConnectionString("Books");

        services.AddDbContext<BooksDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IBookRepository, EfBookRepository>();

        services.AddScoped<IBookService, BookService>();

        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(IBooksModuleMarker)));

        logger.Information("{Module} module services registered", "Books");

        return services;
    }
}