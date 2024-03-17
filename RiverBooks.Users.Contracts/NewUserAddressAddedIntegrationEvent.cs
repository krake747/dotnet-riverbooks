namespace RiverBooks.Users.Contracts;

public sealed record NewUserAddressAddedIntegrationEvent(UserAddressDetails Details) : IntegrationEventBase;