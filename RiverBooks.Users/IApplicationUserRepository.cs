namespace RiverBooks.Users;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetUserWithCartByEmailAsync(string email, CancellationToken token = default);
    Task SaveChangesAsync(CancellationToken token = default);
}