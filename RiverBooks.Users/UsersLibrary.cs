using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;

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
    public string UserId { get; private set; } = string.Empty;
    public Address StreetAddress { get; } = default!;
}

public sealed class ApplicationUser : IdentityUser
{
    private readonly List<UserStreetAddress> _addresses = [];
    private readonly List<CartItem> _cartItems = [];

    public string FullName { get; set; } = string.Empty;
    public IReadOnlyCollection<CartItem> CartItems => _cartItems.AsReadOnly();
    public IReadOnlyCollection<UserStreetAddress> Addresses => _addresses.AsReadOnly();

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
    public Guid BookId { get; private set; }
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