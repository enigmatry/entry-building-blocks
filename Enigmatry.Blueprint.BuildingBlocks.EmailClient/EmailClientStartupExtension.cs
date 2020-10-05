using Enigmatry.Blueprint.BuildingBlocks.Core.Options;
using Enigmatry.Blueprint.BuildingBlocks.Email.MailKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Blueprint.BuildingBlocks.Email
{
    public static class EmailClientStartupExtension
    {
        public static void AppAddEmailClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.AppSmtp));

            var smtpOptions = configuration.GetSection(SmtpOptions.AppSmtp).Get<SmtpOptions>();
            
            if (smtpOptions.UsePickupDirectory)
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
