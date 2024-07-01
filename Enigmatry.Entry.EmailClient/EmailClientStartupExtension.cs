using System;
using Enigmatry.Entry.Core.Settings;
using Enigmatry.Entry.Email.MailKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.Email
{
    public static class EmailClientStartupExtension
    {
        [Obsolete("Use AddEntryEmailClient instead")]
        public static void AppAddEmailClient(this IServiceCollection services, IConfiguration configuration) => services.AddEntryEmailClient(configuration);

        public static void AddEntryEmailClient(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(SmtpSettings.AppSmtp);

            if (!section.Exists())
            {
                throw new InvalidOperationException(
                    $"Section is missing from configuration. Section Name: {SmtpSettings.AppSmtp}");
            }

            services.Configure<SmtpSettings>(section);

            var smtpSettings = section.Get<SmtpSettings>()!;

            if (smtpSettings.UsePickupDirectory)
            {
                services.AddScoped<IEmailClient, MailKitPickupDirectoryEmailClient>();
            }
            else
            {
                services.AddScoped<IEmailClient, MailKitEmailClient>();
            }
        }
    }
}
