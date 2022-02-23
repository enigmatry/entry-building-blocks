using Enigmatry.BuildingBlocks.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.Email.MailKit
{
    internal class MailKitPickupDirectoryEmailClient : IEmailClient
    {
        private readonly ILogger<MailKitPickupDirectoryEmailClient> _logger;
        private readonly SmtpSettings _settings;

        public MailKitPickupDirectoryEmailClient(IOptionsSnapshot<SmtpSettings> optionsSnapshot,
            ILogger<MailKitPickupDirectoryEmailClient> logger)
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
            if (!Directory.Exists(_settings.PickupDirectoryLocation))
            {
                Directory.CreateDirectory(_settings.PickupDirectoryLocation);
            }

            var result = new List<EmailMessageSendResult>();

            foreach (var emailMessage in emailMessages)
            {
                var filePath = Path.Combine(_settings.PickupDirectoryLocation, $"{DateTime.Now.Ticks}.eml");

                using var message = new MimeMessage();
                message.SetEmailData(emailMessage, _settings);

                var sendResult = new EmailMessageSendResult { Message = emailMessage };
                result.Add(sendResult);

                try
                {
                    await message.WriteToAsync(filePath, cancellationToken);
                    _logger.LogInformation(
                        "Message with id {MessageId} and subject '{Subject}' and recipient {To} is written to {FilePath}",
                        message.MessageId, message.Subject, message.To, filePath);
                    sendResult.Success = true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Unable to write message with id {MessageId} and subject '{Subject}' and recipient {To} to {FilePath}",
                        message.MessageId, message.Subject, message.To, filePath);
                }
            }

            return result;
        }
    }
}
