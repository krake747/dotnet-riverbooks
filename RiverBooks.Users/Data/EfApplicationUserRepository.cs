using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.CartEndpoints;

namespace RiverBooks.Users.Data;

internal sealed class EfApplicationUserRepository(UsersDbContext dbContext) : IApplicationUserRepository
{
    public async Task<ApplicationUser?> GetUserWithCartByEmailAsync(string email, CancellationToken token = default)
    {
        return await dbContext.ApplicationUsers
            .Include(user => user.CartItems)
            .SingleAsync(user => user.Email == email, token);
    }

    public async Task SaveChangesAsync(CancellationToken token = default)
    {
        await dbContext.SaveChangesAsync(token);
    }
}