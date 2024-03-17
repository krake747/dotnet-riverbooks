using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Data;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options, IDomainEventDispatcher? dispatcher)
    : IdentityDbContext(options)
{
    public DbSet<ApplicationUser> ApplicationUsers { get; init; } = null!;
    public DbSet<UserStreetAddress> UserStreetAddresses { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Users");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
            .HavePrecision(18, 6);
    }

    /// <summary>
    ///     This is needed for domain events to work
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        var result = await base.SaveChangesAsync(token).ConfigureAwait(false);

        // ignore events if no dispatcher provided
        if (dispatcher is null)
        {
            return result;
        }

        // dispatch events only if save was successful
        var entitiesWithEvents = ChangeTracker.Entries<IHaveDomainEvents>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToArray();

        await dispatcher.DispatchAndClearEvents(entitiesWithEvents);

        return result;
    }
}