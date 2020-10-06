using Enigmatry.Blueprint.BuildingBlocks.Core.Settings;
using Enigmatry.Blueprint.BuildingBlocks.Email.MailKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Blueprint.BuildingBlocks.Email
{
    public static class EmailClientStartupExtension
    {
        public static void AppAddEmailClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.AppSmtp));

            var smtpSettings = configuration.GetSection(SmtpSettings.AppSmtp).Get<SmtpSettings>();
            
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
