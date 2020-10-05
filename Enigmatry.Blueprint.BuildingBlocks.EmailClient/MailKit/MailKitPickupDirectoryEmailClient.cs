using Enigmatry.Blueprint.BuildingBlocks.Core.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.IO;

namespace Enigmatry.Blueprint.BuildingBlocks.Email.MailKit
{
    internal class MailKitPickupDirectoryEmailClient : IEmailClient
    {
        private readonly ILogger<MailKitPickupDirectoryEmailClient> _logger;
        private readonly SmtpOptions _options;

        public MailKitPickupDirectoryEmailClient(IOptionsSnapshot<SmtpOptions> optionsSnapshot, ILogger<MailKitPickupDirectoryEmailClient> logger)
        {
            _options = optionsSnapshot.Value;
            _logger = logger;
        }

        public void Send(EmailMessage email)
        {
            var message = new MimeMessage();
            message.SetEmailData(email, _options);

            if (!Directory.Exists(_options.PickupDirectoryLocation))
            {
                Directory.CreateDirectory(_options.PickupDirectoryLocation);
            }
            var filePath = Path.Combine(_options.PickupDirectoryLocation, $"{DateTime.Now.Ticks}.eml");
            message.WriteTo(filePath);

            _logger.LogDebug($"Email '{message.Subject}' is written to filesystem: {filePath}");
        }
    }
}
