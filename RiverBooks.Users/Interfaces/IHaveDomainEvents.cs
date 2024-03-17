using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Interfaces;

public interface IHaveDomainEvents
{
    IEnumerable<DomainEventBase> DomainEvents { get; }
    void ClearDomainEvents();
}