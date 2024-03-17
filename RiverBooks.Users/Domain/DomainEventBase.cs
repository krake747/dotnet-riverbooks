using MediatR;

namespace RiverBooks.Users.Domain;

public abstract class DomainEventBase : INotification
{
    public DateTime DateOccured { get; protected set; } = DateTime.UtcNow;
}