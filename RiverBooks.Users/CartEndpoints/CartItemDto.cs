namespace RiverBooks.Users.CartEndpoints;

public sealed record CartItemDto(
    Guid Id, 
    Guid BookId, 
    string Description, 
    int Quantity, 
    decimal UnitPrice);