using Enigmatry.Blueprint.BuildingBlocks.Core.Settings;
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
        private readonly SmtpSettings _settings;

        public MailKitPickupDirectoryEmailClient(IOptionsMonitor<SmtpSettings> optionsMonitor, ILogger<MailKitPickupDirectoryEmailClient> logger)
        {
            _settings = optionsMonitor.CurrentValue;
            _logger = logger;
        }

        public void Send(EmailMessage email)
        {
            var message = new MimeMessage();
            message.SetEmailData(email, _settings);

            if (!Directory.Exists(_settings.PickupDirectoryLocation))
            {
                Directory.CreateDirectory(_settings.PickupDirectoryLocation);
            }
            var filePath = Path.Combine(_settings.PickupDirectoryLocation, $"{DateTime.Now.Ticks}.eml");
            message.WriteTo(filePath);

            _logger.LogDebug($"Email '{message.Subject}' is written to filesystem: {filePath}");
        }
    }
}
