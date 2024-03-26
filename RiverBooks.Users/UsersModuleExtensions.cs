using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Riverbooks.SharedKernel;
using RiverBooks.Users.Data;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Infrastructure.Data;
using RiverBooks.Users.Interfaces;
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

        services.AddScoped<IApplicationUserRepository, EfApplicationUserRepository>();
        services.AddScoped<IReadOnlyUserStreetAddressRepository, EfUserStreetAddressRepository>();

        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(IUsersModuleMarker)));

        services.AddFluentValidationValidators<IUsersModuleMarker>();

        logger.Information("{Module} module services registered", "Users");

        return services;
    }
}