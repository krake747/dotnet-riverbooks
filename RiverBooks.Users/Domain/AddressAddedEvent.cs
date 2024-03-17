namespace RiverBooks.Users.Domain;

internal sealed class AddressAddedEvent(UserStreetAddress newAddress) : DomainEventBase
{
    public UserStreetAddress NewAddress { get; } = newAddress;
}