using System;
using System.Diagnostics;
using JetBrains.Annotations;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Enigmatry.Blueprint.BuildingBlocks.Email.MailKit
{
    [UsedImplicitly]
    public class MailKitEmailClient : IEmailClient
    {
        private readonly ILogger<MailKitEmailClient> _logger;
        private readonly SmtpSettings _settings;

        public MailKitEmailClient(SmtpSettings settings, ILogger<MailKitEmailClient> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public void Send(EmailMessage email)
        {
            var message = new MimeMessage();
            message.SetEmailData(email, _settings);

            var stopWatch = new Stopwatch();

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(_settings.Server, _settings.Port);
                if (!String.IsNullOrEmpty(_settings.Username) && !String.IsNullOrEmpty(_settings.Password))
                {
                    smtpClient.Authenticate(_settings.Username, _settings.Password);
                }
                smtpClient.Send(message);
                smtpClient.Disconnect(true);
            }

            _logger.LogDebug($"Email {message.Subject}, using host: {_settings.Server} and port: {_settings.Port}, sent in [{stopWatch.ElapsedMilliseconds}ms]");
        }
    }
}
