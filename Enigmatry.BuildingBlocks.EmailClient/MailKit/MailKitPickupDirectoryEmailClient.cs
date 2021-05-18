using Enigmatry.BuildingBlocks.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.Email.MailKit
{
    internal class MailKitPickupDirectoryEmailClient : IEmailClient
    {
        private readonly ILogger<MailKitPickupDirectoryEmailClient> _logger;
        private readonly SmtpSettings _settings;

        public MailKitPickupDirectoryEmailClient(IOptionsSnapshot<SmtpSettings> optionsSnapshot, ILogger<MailKitPickupDirectoryEmailClient> logger)
        {
            _settings = optionsSnapshot.Value;
            _logger = logger;
        }

        public async Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default) =>
            await SendBulkAsync(emailMessage.GetBulk(), cancellationToken);

        public async Task SendBulkAsync(IEnumerable<EmailMessage> emailMessages, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(_settings.PickupDirectoryLocation))
            {
                Directory.CreateDirectory(_settings.PickupDirectoryLocation);
            }

            foreach (var mail in emailMessages)
            {
                var filePath = Path.Combine(_settings.PickupDirectoryLocation, $"{DateTime.Now.Ticks}.eml");

                var message = new MimeMessage();
                message.SetEmailData(mail, _settings);

                await message.WriteToAsync(filePath, cancellationToken);

                _logger.LogDebug($"Email '{message.Subject}' is written to filesystem: {filePath}");
            }

            await Task.CompletedTask;
        }
    }
}
