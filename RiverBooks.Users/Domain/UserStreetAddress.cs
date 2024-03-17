using Ardalis.GuardClauses;

namespace RiverBooks.Users.Domain;

public sealed class UserStreetAddress
{
    private UserStreetAddress()
    {
        // EF
    }

    public UserStreetAddress(string userId, Address streetAddress)
    {
        UserId = Guard.Against.NullOrEmpty(userId);
        StreetAddress = Guard.Against.Null(streetAddress);
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string UserId { get; } = string.Empty;
    public Address StreetAddress { get; } = default!;
}