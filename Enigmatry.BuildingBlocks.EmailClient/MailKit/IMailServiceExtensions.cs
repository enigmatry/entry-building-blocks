using System;
using System.Threading;
using System.Threading.Tasks;
using Enigmatry.BuildingBlocks.Core.Settings;
using MailKit;
using MailKit.Security;

namespace Enigmatry.BuildingBlocks.Email.MailKit
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

            if (!String.IsNullOrEmpty(settings.Username) && !String.IsNullOrEmpty(settings.Password))
            {
                await mailService.AuthenticateAsync(settings.Username, settings.Password, cancellationToken);
            }
        }
    }
}
