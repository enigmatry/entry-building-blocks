using System;
using System.Threading;
using System.Threading.Tasks;
using Enigmatry.Entry.Core.Settings;
using MailKit;
using MailKit.Security;

namespace Enigmatry.Entry.Email.MailKit
{
    internal static class IMailServiceExtensions
    {
        internal static async Task ConnectAsync(this IMailService mailService, SmtpSettings settings, CancellationToken cancellationToken = default)
        {
            if (settings.Port == 465)
            {
                await mailService.ConnectAsync(settings.Server, settings.Port, true, cancellationToken);
            }
            else
            {
                // To support smtp over non-standard SSL ports (like Office365, which uses port 587)
                await mailService.ConnectAsync(settings.Server, settings.Port, SecureSocketOptions.StartTls, cancellationToken);
            }

            if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
            {
                await mailService.AuthenticateAsync(settings.Username, settings.Password, cancellationToken);
            }
        }
    }
}
