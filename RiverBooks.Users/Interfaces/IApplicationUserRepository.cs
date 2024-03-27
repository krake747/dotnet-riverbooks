using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Interfaces;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId, CancellationToken token = default);
    Task<ApplicationUser?> GetUserWithCartByEmailAsync(string email, CancellationToken token = default);
    Task<ApplicationUser?> GetUserWithAddressesByEmailAsync(string email, CancellationToken token = default);
    Task SaveChangesAsync(CancellationToken token = default);
}