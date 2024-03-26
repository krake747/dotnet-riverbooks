using Ardalis.Result;
using MediatR;

namespace RiverBooks.EmailSending;

internal sealed class SendEmailCommandHandler(ISendEmail emailSender) : IRequestHandler<SendEmailCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SendEmailCommand request, CancellationToken token = default)
    {
        await emailSender.SendEmailAsync(request.To, request.From, request.Subject, request.Body, token);

        return Guid.Empty;
    }
}