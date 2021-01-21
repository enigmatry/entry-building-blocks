using System;
using System.Threading;
using System.Threading.Tasks;
using Enigmatry.Blueprint.BuildingBlocks.Core.Settings;
using MailKit;

namespace Enigmatry.Blueprint.BuildingBlocks.Email.MailKit
{
    internal static class IMailServiceExtensions
    {
        internal static async Task ConnectAsync(this IMailService mailService, SmtpSettings settings, CancellationToken cancellationToken = default)
        {
            await mailService.ConnectAsync(settings.Server, settings.Port, true, cancellationToken);
            if (!String.IsNullOrEmpty(settings.Username) && !String.IsNullOrEmpty(settings.Password))
            {
                await mailService.AuthenticateAsync(settings.Username, settings.Password, cancellationToken);
            }
        }
    }
}
