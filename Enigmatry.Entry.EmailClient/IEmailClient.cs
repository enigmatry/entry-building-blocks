namespace Enigmatry.Entry.Email;

public interface IEmailClient
{
    public Task<EmailMessageSendResult> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    public Task<IEnumerable<EmailMessageSendResult>> SendBulkAsync(IEnumerable<EmailMessage> emailMessages, CancellationToken cancellationToken = default);
}
