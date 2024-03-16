using Microsoft.EntityFrameworkCore;

namespace RiverBooks.OrderProcessing.Data;

internal sealed class EfOrderRepository(OrderProcessingDbContext dbContext) : IOrderRepository
{
    public async Task<List<Order>> ListAsync(CancellationToken token = default) =>
        await dbContext.Orders
            .Include(o => o.OrderItems)
            .ToListAsync(token);

    public async Task AddAsync(Order order, CancellationToken token = default) => 
        await dbContext.AddAsync(order, token);

    public async Task SaveChangesAsync(CancellationToken token = default) => 
        await dbContext.SaveChangesAsync(token);
    
}