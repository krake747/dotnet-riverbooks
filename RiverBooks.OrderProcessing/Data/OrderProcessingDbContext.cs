using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RiverBooks.OrderProcessing.Data;

public sealed class OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options)
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
}