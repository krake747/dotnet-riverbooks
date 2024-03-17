namespace RiverBooks.Users;

public interface IReadOnlyUserStreetAddressRepository
{
    Task<UserStreetAddress?> GetById(Guid userStreetAddressId, CancellationToken token = default);
}