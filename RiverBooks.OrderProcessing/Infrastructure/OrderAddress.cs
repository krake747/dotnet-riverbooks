using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Infrastructure;

internal sealed record OrderAddress(Guid Id, Address Address);