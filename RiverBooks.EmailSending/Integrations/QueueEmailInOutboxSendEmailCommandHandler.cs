using Ardalis.Result;
using MediatR;
using MongoDB.Driver;
using RiverBooks.EmailSending.Contracts;

namespace RiverBooks.EmailSending.Integrations;

internal interface IQueueEmailsInOutboxService
{
    Task QueueEmailForSending(EmailOutboxEntity entity, CancellationToken token = default);
}

internal sealed class MongoDbQueueEmailsInOutboxService(IMongoCollection<EmailOutboxEntity> emailCollection)
    : IQueueEmailsInOutboxService
{
    public async Task QueueEmailForSending(EmailOutboxEntity entity, CancellationToken token = default)
    {
        await emailCollection.InsertOneAsync(entity, cancellationToken: token);
    }
}

internal sealed class QueueEmailInOutboxSendEmailCommandHandler(IQueueEmailsInOutboxService outboxService) 
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