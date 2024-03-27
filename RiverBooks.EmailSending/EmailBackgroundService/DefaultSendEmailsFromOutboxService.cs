using Ardalis.Result;
using MongoDB.Driver;
using Serilog;

namespace RiverBooks.EmailSending.EmailBackgroundService;

internal interface IGetEmailsFromOutboxService
{
    Task<Result<EmailOutboxEntity>> GetUnprocessedEmailEntity(CancellationToken token = default);
}

internal sealed class MongoDbGetEmailsFromOutboxService(IMongoCollection<EmailOutboxEntity> emailCollection) 
    : IGetEmailsFromOutboxService
{
    public async Task<Result<EmailOutboxEntity>> GetUnprocessedEmailEntity(CancellationToken token = default)
    {
        var filter = Builders<EmailOutboxEntity>.Filter.Eq(e => e.DateTimeUtcProcessed, null);
        var unsentEmailEntity = await emailCollection.Find(filter).FirstOrDefaultAsync(token);

        if (unsentEmailEntity is null)
        {
            return Result.NotFound();
        }

        return unsentEmailEntity;
    }
}

internal sealed class DefaultSendEmailsFromOutboxService(ILogger logger, IGetEmailsFromOutboxService outboxService, 
    ISendEmail emailSender, IMongoCollection<EmailOutboxEntity> emailCollection) 
    : ISendEmailsFromOutboxService
{
    private readonly ILogger _logger = logger.ForContext<DefaultSendEmailsFromOutboxService>();
    
    public async Task CheckForAndSendEmailsAsync()
    {
        try
        {
            var result = await outboxService.GetUnprocessedEmailEntity();

            if (result.Status is ResultStatus.NotFound)
            {
                return;
            }

            var emailEntity = result.Value;

            await emailSender.SendEmailAsync(emailEntity.To,
                emailEntity.From,
                emailEntity.Subject,
                emailEntity.Body);

            var updateFilter = Builders<EmailOutboxEntity>.Filter.Eq(x => x.Id, emailEntity.Id);
            var update = Builders<EmailOutboxEntity>.Update.Set("DateTimeUtcProcessed", DateTime.UtcNow);
            var updateResult = await emailCollection.UpdateOneAsync(updateFilter, update);
            
            _logger.Information("Processed {Result} emails records", updateResult.ModifiedCount);
        }
        finally
        {
            _logger.Information("Sleeping...");
        }
    }
}