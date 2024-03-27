using FastEndpoints;
using MongoDB.Driver;

namespace RiverBooks.EmailSending.Endpoints;

internal sealed class ListEmails(IMongoCollection<EmailOutboxEntity> emailCollection) : EndpointWithoutRequest<ListEmailsResponse>
{
    public override void Configure()
    {
        Get("/emails");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken token)
    {
        // TODO: Implement paging
        var filter = Builders<EmailOutboxEntity>.Filter.Empty;
        var emailEntities = await emailCollection.Find(filter).ToListAsync(token);

        var response = new ListEmailsResponse
        {
            Count = emailEntities.Count,
            Emails = emailEntities // TODO: Use a separate DTO
        };
        
        await SendOkAsync(response, token);
    }
}

internal sealed class ListEmailsResponse
{
    public int Count { get; set; }
    public List<EmailOutboxEntity> Emails { get; internal set; } = [];
}