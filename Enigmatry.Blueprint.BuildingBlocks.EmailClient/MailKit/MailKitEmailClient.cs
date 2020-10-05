using System;
using System.Diagnostics;
using Enigmatry.Blueprint.BuildingBlocks.Core.Options;
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
        private readonly SmtpOptions _options;

        public MailKitEmailClient(IOptionsSnapshot<SmtpOptions> optionsSnapshot, ILogger<MailKitEmailClient> logger)
        {
            _options = optionsSnapshot.Value;
            _logger = logger;
        }

        public void Send(EmailMessage email)
        {
            var message = new MimeMessage();
            message.SetEmailData(email, _options);

            var stopWatch = new Stopwatch();

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(_options.Server, _options.Port);
                if (!String.IsNullOrEmpty(_options.Username) && !String.IsNullOrEmpty(_options.Password))
                {
                    smtpClient.Authenticate(_options.Username, _options.Password);
                }
                smtpClient.Send(message);
                smtpClient.Disconnect(true);
            }

            _logger.LogDebug($"Email {message.Subject}, using host: {_options.Server} and port: {_options.Port}, sent in [{stopWatch.ElapsedMilliseconds}ms]");
        }
    }
}
