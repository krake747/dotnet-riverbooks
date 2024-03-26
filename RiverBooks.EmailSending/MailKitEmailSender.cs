using MailKit.Net.Smtp;
using MimeKit;
using Serilog;

namespace RiverBooks.EmailSending;

public sealed class MimeKitEmailSender(ILogger logger) : ISendEmail
{
    private readonly ILogger _logger = logger.ForContext<MimeKitEmailSender>();
    public async Task SendEmailAsync(string to, string from, string subject, string body, 
        CancellationToken token = default)
    {
        _logger.Information("Attempting to send email to {To} from {From} with subject {Subject}...", to, from, subject);

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(Constants.EmailServer, 25, false, token); // TODO: fetch from config
        
        var message = new MimeMessage
        {
            From = { new MailboxAddress(from, from) },
            To = { new MailboxAddress(to, to) },
            Subject = subject,
            Body = new TextPart("plain") { Text = body }
        };

        await smtp.SendAsync(message, token);
        _logger.Information("Email sent!");

        await smtp.DisconnectAsync(true, token);
    }
}