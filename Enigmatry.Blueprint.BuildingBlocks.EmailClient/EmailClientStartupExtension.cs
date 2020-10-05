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
            var smtpOptions = configuration.GetSection("App:Smtp").Get<SmtpOptions>();
            
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
