using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Data;

internal sealed class EfUserStreetAddressRepository(UsersDbContext dbContext)
    : IReadOnlyUserStreetAddressRepository
{
    public Task<UserStreetAddress?> GetById(Guid userStreetAddressId, CancellationToken token = default) =>
        dbContext.UserStreetAddresses
            .SingleOrDefaultAsync(a => a.Id == userStreetAddressId, token);
}