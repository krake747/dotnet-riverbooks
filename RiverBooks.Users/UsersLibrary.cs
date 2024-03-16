using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;

namespace RiverBooks.Users;

public sealed class ApplicationUser : IdentityUser
{
    private readonly List<CartItem> _cartItems = [];

    public string FullName { get; set; } = string.Empty;
    public IReadOnlyCollection<CartItem> CartItems => _cartItems.AsReadOnly();

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