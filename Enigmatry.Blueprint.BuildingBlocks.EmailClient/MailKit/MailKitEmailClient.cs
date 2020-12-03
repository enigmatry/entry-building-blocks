using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enigmatry.Blueprint.BuildingBlocks.Core.Settings;
using JetBrains.Annotations;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Enigmatry.Blueprint.BuildingBlocks.Email.MailKit
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

        public async Task SendAsync(EmailMessage email)
        {
            var numberOfSentEmails = 0;
            var stopWatch = new Stopwatch();

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_settings.Server, _settings.Port);
                if (!String.IsNullOrEmpty(_settings.Username) && !String.IsNullOrEmpty(_settings.Password))
                {
                    await smtpClient.AuthenticateAsync(_settings.Username, _settings.Password);
                }

                foreach (var mail in email.GetBulk())
                {
                    var message = new MimeMessage();
                    message.SetEmailData(mail, _settings);
                    message.Cc.AddRange(mail.Cc.Select(x => new MailboxAddress(Encoding.UTF8, x.DisplayName, x.Address)));

                    if (message.To.Count != 1)
                        throw new InvalidOperationException($"'{nameof(message.To)}' is allowed to contain only one email address");

                    await smtpClient.SendAsync(message);
                    numberOfSentEmails++;
                }

                await smtpClient.DisconnectAsync(true);
            }

            _logger.LogDebug($"{numberOfSentEmails} email(s) {email.Subject}, using host: {_settings.Server} and port: {_settings.Port}, sent in [{stopWatch.ElapsedMilliseconds}ms]");
        }
    }
}
