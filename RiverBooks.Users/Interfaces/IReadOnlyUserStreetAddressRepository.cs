using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Interfaces;

public interface IReadOnlyUserStreetAddressRepository
{
    Task<UserStreetAddress?> GetById(Guid userStreetAddressId, CancellationToken token = default);
}