using System.Diagnostics;
using Enigmatry.Entry.Core.Settings;
using JetBrains.Annotations;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Enigmatry.Entry.Email.MailKit;

[UsedImplicitly]
internal class MailKitEmailClient : IEmailClient
{
    private readonly ILogger<MailKitEmailClient> _logger;
    private readonly SmtpSettings _settings;

    public MailKitEmailClient(IOptionsSnapshot<SmtpSettings> optionsSnapshot, ILogger<MailKitEmailClient> logger)
    {
        _settings = optionsSnapshot.Value;
        _logger = logger;
    }

    public async Task<EmailMessageSendResult> SendAsync(EmailMessage emailMessage,
        CancellationToken cancellationToken = default) =>
        (await SendBulkAsync(emailMessage.GetBulk(), cancellationToken)).First();

    public async Task<IEnumerable<EmailMessageSendResult>> SendBulkAsync(IEnumerable<EmailMessage> emailMessages,
        CancellationToken cancellationToken = default)
    {
        if (emailMessages == null)
        {
            throw new ArgumentNullException(nameof(emailMessages));
        }

        var numberOfSentEmails = 0;
        var stopWatch = new Stopwatch();
        var result = new List<EmailMessageSendResult>();

        using (var smtpClient = new SmtpClient())
        {
            await smtpClient.ConnectAsync(_settings, cancellationToken);

            foreach (var emailMessage in emailMessages)
            {
                var sendResult = new EmailMessageSendResult { Message = emailMessage };
                result.Add(sendResult);
                try
                {
                    using var message = new MimeMessage();
                    message.SetEmailData(emailMessage, _settings);

                    var response = await smtpClient.SendAsync(message, cancellationToken);
                    _logger.LogInformation(
                        "Message with id {MessageId} sent with server response {ServerResponse}",
                        emailMessage.MessageId, response);
                    numberOfSentEmails++;
                    sendResult.Success = true;
                }
                catch (SmtpCommandException ex)
                {
                    _logger.LogWarning(ex,
                        "Unable to send message with id {MessageId} with error code {ErrorCode} due to a command error: {ErrorMessage}",
                        emailMessage.MessageId, ex.ErrorCode, ex.Message);
                }
                catch (ProtocolException ex) when (smtpClient.IsConnected)
                {
                    // only when the client is still connected we should continue trying to send any other message
                    _logger.LogWarning(ex,
                        "Unable to send message with id {MessageId} due to a protocol error: {ErrorMessage}",
                        emailMessage.MessageId, ex.Message);
                }
            }

            await smtpClient.DisconnectAsync(true, cancellationToken);
        }

        _logger.LogDebug(
            $"{numberOfSentEmails} email(s) using host: {_settings.Server} and port: {_settings.Port}, sent in [{stopWatch.ElapsedMilliseconds}ms]");

        return result;
    }
}
