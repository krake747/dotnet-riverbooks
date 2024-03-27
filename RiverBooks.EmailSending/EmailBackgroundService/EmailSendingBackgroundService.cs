using Microsoft.Extensions.Hosting;
using Serilog;

namespace RiverBooks.EmailSending.EmailBackgroundService;

internal sealed class EmailSendingBackgroundService(ILogger logger, 
    ISendEmailsFromOutboxService sendEmailsFromOutboxService) : BackgroundService
{
    private readonly ILogger _logger = logger.ForContext<EmailSendingBackgroundService>();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int delayMilliseconds = 10_000; // 10 seconds
        _logger.Information("{ServiceName} starting...", nameof(EmailSendingBackgroundService));

        while (stoppingToken.IsCancellationRequested is false)
        {
            try
            {
                await sendEmailsFromOutboxService.CheckForAndSendEmailsAsync();
            }
            catch (Exception ex)
            {
                _logger.Error("Error processing outbox: {Message}", ex.Message);
            }
            finally
            {
                await Task.Delay(delayMilliseconds, stoppingToken);
            }
            
            _logger.Information("{ServiceName} stopping", nameof(EmailSendingBackgroundService));
        }
    }
}