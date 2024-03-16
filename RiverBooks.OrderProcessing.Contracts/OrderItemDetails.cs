namespace RiverBooks.OrderProcessing.Contracts;

public sealed record OrderItemDetails(
    Guid BookId,
    int Quantity,
    decimal UnitPrice,
    string Description);