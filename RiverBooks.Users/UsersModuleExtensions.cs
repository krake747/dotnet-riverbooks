using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RiverBooks.Users;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, ConfigurationManager config,
        ILogger logger)
    {
        var connectionString = config.GetConnectionString("Users");
        services.AddDbContext<UsersDbContext>(options => options.UseSqlServer(connectionString));

        services.AddIdentityCore<ApplicationUser>()
            .AddEntityFrameworkStores<UsersDbContext>();

        logger.Information("{Module} module services registered", "Users");

        return services;
    }
}
