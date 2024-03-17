using System.ComponentModel.DataAnnotations.Schema;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace RiverBooks.Users;

public sealed record Address(
    string Street1,
    string Street2,
    string City,
    string State,
    string PostalCode,
    string Country);

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

public interface IHaveDomainEvents
{
    IEnumerable<DomainEventBase> DomainEvents { get; }
    void ClearDomainEvents();
}

public abstract class DomainEventBase : INotification
{
    public DateTime DateOccured { get; protected set; } = DateTime.UtcNow;
}

internal sealed class AddressAddedEvent(UserStreetAddress newAddress) : DomainEventBase
{
    public UserStreetAddress NewAddress { get; } = newAddress;
}

internal sealed class LogNewAddressHandler(ILogger logger) : INotificationHandler<AddressAddedEvent>
{
    public Task Handle(AddressAddedEvent notification, CancellationToken token = default)
    {
        logger.Information("[DE Handler]New address added to user {User}: {Address}",
            notification.NewAddress.UserId,
            notification.NewAddress.StreetAddress);

        return Task.CompletedTask;
    }
}

public interface IDomainEventDispatcher
{
    Task DispatchAndClearEvents(IEnumerable<IHaveDomainEvents> entitiesWithEvents);
}

public sealed class MediatorDomainEventDispatcher(IPublisher mediator) : IDomainEventDispatcher
{
    public async Task DispatchAndClearEvents(IEnumerable<IHaveDomainEvents> entitiesWithEvents)
    {
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            foreach (var domainEvent in events)
            {
                await mediator.Publish(domainEvent).ConfigureAwait(false);
            }
        }
    }
}

public sealed class ApplicationUser : IdentityUser, IHaveDomainEvents
{
    private readonly List<UserStreetAddress> _addresses = [];
    private readonly List<CartItem> _cartItems = [];
    private readonly List<DomainEventBase> _domainEvents = [];

    public string FullName { get; set; } = string.Empty;
    public IReadOnlyCollection<CartItem> CartItems => _cartItems.AsReadOnly();
    public IReadOnlyCollection<UserStreetAddress> Addresses => _addresses.AsReadOnly();

    [NotMapped] public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();
    void IHaveDomainEvents.ClearDomainEvents() => _domainEvents.Clear();

    internal UserStreetAddress AddAddress(Address address)
    {
        Guard.Against.Null(address);

        var existingAddress = _addresses.SingleOrDefault(a => a.StreetAddress == address);
        if (existingAddress is not null)
        {
            return existingAddress;
        }

        var newAddress = new UserStreetAddress(Id, address);
        _addresses.Add(newAddress);

        var domainEvent = new AddressAddedEvent(newAddress);
        RegisterDomainEvent(domainEvent);

        return newAddress;
    }

    public void AddItemToCart(CartItem item)
    {
        Guard.Against.Null(item);

        var existingBook = _cartItems.Find(c => c.BookId == item.BookId);
        if (existingBook is not null)
        {
            existingBook.UpdateQuantity(existingBook.Quantity + item.Quantity);
            existingBook.UpdateDescription(item.Description);
            existingBook.UpdateUnitPrice(item.UnitPrice);
            return;
        }

        _cartItems.Add(item);
    }

    internal void ClearCart() => _cartItems.Clear();

    private void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
}

public sealed class CartItem
{
    public CartItem(Guid bookId, string description, int quantity, decimal unitPrice)
    {
        BookId = Guard.Against.Default(bookId);
        Description = Guard.Against.NullOrEmpty(description);
        Quantity = Guard.Against.Negative(quantity);
        UnitPrice = Guard.Against.Negative(unitPrice);
    }

    private CartItem()
    {
        // EF
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookId { get; }
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    internal void UpdateQuantity(int quantity) =>
        Quantity = Guard.Against.Negative(quantity);

    internal void UpdateDescription(string description) =>
        Description = Guard.Against.NullOrEmpty(description);

    internal void UpdateUnitPrice(decimal unitPrice) =>
        UnitPrice = Guard.Against.Negative(unitPrice);
}