using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Data;

internal sealed class EfApplicationUserRepository(UsersDbContext dbContext) : IApplicationUserRepository
{
    public async Task<ApplicationUser?> GetUserWithCartByEmailAsync(string email, CancellationToken token = default) =>
        await dbContext.ApplicationUsers
            .Include(user => user.CartItems)
            .SingleAsync(user => user.Email == email, token);

    public async Task<ApplicationUser?> GetUserWithAddressesByEmailAsync(string email,
        CancellationToken token = default) =>
        await dbContext.ApplicationUsers
            .Include(user => user.Addresses)
            .SingleAsync(user => user.Email == email, token);

    public async Task SaveChangesAsync(CancellationToken token = default) =>
        await dbContext.SaveChangesAsync(token);
}