namespace RiverBooks.EmailSending.EmailBackgroundService;

internal interface ISendEmailsFromOutboxService
{
    Task CheckForAndSendEmailsAsync();
}