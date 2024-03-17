using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing;

public interface IOrderRepository
{
    Task<List<Order>> ListAsync(CancellationToken token = default);
    Task AddAsync(Order order, CancellationToken token = default);
    Task SaveChangesAsync(CancellationToken token = default);
}