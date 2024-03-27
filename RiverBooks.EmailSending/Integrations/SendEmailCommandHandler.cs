using Ardalis.Result;
using RiverBooks.EmailSending.Contracts;

namespace RiverBooks.EmailSending.Integrations;

// Disabled and use Outbox pattern command handler
internal sealed class SendEmailCommandHandler(ISendEmail emailSender) //: IRequestHandler<SendEmailCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SendEmailCommand request, CancellationToken token = default)
    {
        await emailSender.SendEmailAsync(request.To, request.From, request.Subject, request.Body, token);

        return Guid.Empty;
    }
}