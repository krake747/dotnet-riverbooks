using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RiverBooks.OrderProcessing.Domain;
using Riverbooks.SharedKernel;

namespace RiverBooks.OrderProcessing.Infrastructure.Data;

public sealed class OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options, 
    IDomainEventDispatcher? dispatcher)
    : IdentityDbContext(options)
{
    public DbSet<Order> Orders { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("OrderProcessing");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderProcessingDbContext).Assembly);

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