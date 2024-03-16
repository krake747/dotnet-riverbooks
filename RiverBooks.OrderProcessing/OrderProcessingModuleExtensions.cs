using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.OrderProcessing.Data;
using Serilog;

namespace RiverBooks.OrderProcessing;

public static class OrderProcessingModuleExtensions
{
    public static IServiceCollection AddOrderProcessingModule(this IServiceCollection services, ConfigurationManager config,
        ILogger logger)
    {
        var connectionString = config.GetConnectionString("OrderProcessing");
        services.AddDbContext<OrderProcessingDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IOrderRepository, EfOrderRepository>();

        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(IOrderProcessingModuleMarker)));

        logger.Information("{Module} module services registered", "Users");

        return services;
    }
}