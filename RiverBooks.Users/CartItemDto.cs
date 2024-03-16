namespace RiverBooks.Users;

public sealed record CartItemDto(Guid Id, Guid BookId, string Description, int Quantity, decimal UnitPrice);