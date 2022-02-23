using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enigmatry.BuildingBlocks.Core.Settings;
using JetBrains.Annotations;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Enigmatry.BuildingBlocks.Email.MailKit
{
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
                            "Message with id {MessageId} and subject {Subject} sent from {From} to {To} with server response {ServerResponse}",
                            emailMessage.MessageId, emailMessage.Subject, emailMessage.From, emailMessage.To, response);
                        numberOfSentEmails++;
                        sendResult.Success = true;
                    }
                    catch (SmtpCommandException ex)
                    {
                        _logger.LogWarning(ex,
                            "Unable to send message with id {MessageId} and subject {Subject} from {From} to {To} with error code {ErrorCode} due to a command error: {ErrorMessage}",
                            emailMessage.MessageId, emailMessage.Subject, emailMessage.From, emailMessage.To,
                            ex.ErrorCode, ex.Message);
                    }
                    catch (ProtocolException ex) when (smtpClient.IsConnected)
                    {
                        // only when the client is still connected we should continue trying to send any other message
                        _logger.LogWarning(ex,
                            "Unable to send message with id {MessageId} and subject {Subject} from {From} to {To} due to a protocol error: {ErrorMessage}",
                            emailMessage.MessageId, emailMessage.Subject, emailMessage.From, emailMessage.To,
                            ex.Message);
                    }
                }

                await smtpClient.DisconnectAsync(true, cancellationToken);
            }

            _logger.LogDebug(
                $"{numberOfSentEmails} email(s) using host: {_settings.Server} and port: {_settings.Port}, sent in [{stopWatch.ElapsedMilliseconds}ms]");

            return result;
        }
    }
}
