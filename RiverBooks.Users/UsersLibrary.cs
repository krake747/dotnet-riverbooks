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
            // TODO: What to do if other details of the item have been updated?
            return;
        }

        _cartItems.Add(item);
    }
}

public sealed class CartItem(Guid bookId, string description, int quantity, decimal unitPrice)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookId { get; } = Guard.Against.Default(bookId);
    public string Description { get; private set; } = Guard.Against.NullOrEmpty(description);
    public int Quantity { get; private set; } = Guard.Against.Negative(quantity);
    public decimal UnitPrice { get; private set; } = Guard.Against.Negative(unitPrice);

    internal void UpdateQuantity(int quantity)
    {
        Quantity = Guard.Against.Negative(quantity);
    }
}