using MongoDB.Driver;

namespace RiverBooks.EmailSending;

internal sealed class MongoDbOutboxService(IMongoCollection<EmailOutboxEntity> emailCollection) : IOutboxService
{
    public async Task QueueEmailForSending(EmailOutboxEntity entity, CancellationToken token = default)
    {
        await emailCollection.InsertOneAsync(entity, cancellationToken: token);
    }
}