namespace Riverbooks.SharedKernel;

public interface IHaveDomainEvents
{
    IEnumerable<DomainEventBase> DomainEvents { get; }
    void ClearDomainEvents();
}