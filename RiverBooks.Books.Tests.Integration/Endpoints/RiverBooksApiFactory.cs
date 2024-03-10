using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Api;
using RiverBooks.Books.Data;

namespace RiverBooks.Books.Tests.Integration.Endpoints;

public sealed class RiverBooksApiFactory : WebApplicationFactory<IRiverBooksApiMarker>
{
    private readonly IConfiguration _config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.Testing.json")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseConfiguration(_config);
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<BookDbContext>(options => options.UseSqlServer(_config.GetConnectionString("Books")));
        });
    }
}