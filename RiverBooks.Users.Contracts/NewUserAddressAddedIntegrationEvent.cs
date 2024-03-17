using Riverbooks.SharedKernel;

namespace RiverBooks.Users.Contracts;

public sealed record NewUserAddressAddedIntegrationEvent(UserAddressDetails Details) : IntegrationEventBase;