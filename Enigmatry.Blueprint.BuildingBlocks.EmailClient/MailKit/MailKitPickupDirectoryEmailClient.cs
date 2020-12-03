using Enigmatry.Blueprint.BuildingBlocks.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigmatry.Blueprint.BuildingBlocks.Email.MailKit
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

        public async Task SendAsync(EmailMessage email)
        {
            if (!Directory.Exists(_settings.PickupDirectoryLocation))
            {
                Directory.CreateDirectory(_settings.PickupDirectoryLocation);
            }

            foreach (var mail in email.GetBulk())
            {
                var filePath = Path.Combine(_settings.PickupDirectoryLocation, $"{DateTime.Now.Ticks}.eml");

                var message = new MimeMessage();
                message.SetEmailData(mail, _settings);
                message.Cc.AddRange(mail.Cc.Select(x => new MailboxAddress(Encoding.UTF8, x.DisplayName, x.Address)));

                if (message.To.Count != 1)
                    throw new InvalidOperationException($"'{nameof(message.To)}' is allowed to contain only one email address");

                await message.WriteToAsync(filePath);

                _logger.LogDebug($"Email '{message.Subject}' is written to filesystem: {filePath}");
            }

            await Task.CompletedTask;
        }
    }
}
