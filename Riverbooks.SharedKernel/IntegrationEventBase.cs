using MediatR;

namespace Riverbooks.SharedKernel;

public abstract record IntegrationEventBase : INotification
{
    public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.UtcNow;
}