using Ardalis.Result;
using MediatR;
using RiverBooks.EmailSending.Contracts;

namespace RiverBooks.EmailSending.Integrations;

internal sealed class QueueEmailInOutboxSendEmailCommandHandler(IOutboxService outboxService) 
    : IRequestHandler<SendEmailCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SendEmailCommand request, CancellationToken token = default)
    {
        var newEntity = new EmailOutboxEntity
        {
            Body = request.Body,
            Subject = request.Subject,
            To = request.To,
            From = request.From,
        };
        
        await outboxService.QueueEmailForSending(newEntity, token);

        return newEntity.Id;
    }
}