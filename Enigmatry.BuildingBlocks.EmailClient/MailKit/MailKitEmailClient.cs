using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Enigmatry.BuildingBlocks.Core.Settings;
using JetBrains.Annotations;
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

        public async Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default) =>
            await SendBulkAsync(emailMessage.GetBulk(), cancellationToken);

        public async Task SendBulkAsync(IEnumerable<EmailMessage> emailMessages, CancellationToken cancellationToken = default)
        {
            var numberOfSentEmails = 0;
            var stopWatch = new Stopwatch();

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_settings, cancellationToken);

                foreach (var emailMessage in emailMessages)
                {
                    var message = new MimeMessage();
                    message.SetEmailData(emailMessage, _settings);

                    await smtpClient.SendAsync(message, cancellationToken);
                    numberOfSentEmails++;
                }

                await smtpClient.DisconnectAsync(true, cancellationToken);
            }

            _logger.LogDebug($"{numberOfSentEmails} email(s) using host: {_settings.Server} and port: {_settings.Port}, sent in [{stopWatch.ElapsedMilliseconds}ms]");
        }
    }
}
