using Enigmatry.Blueprint.BuildingBlocks.Core.Settings;
using Enigmatry.Blueprint.BuildingBlocks.Email;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Blueprint.BuildingBlocks.Tests.Infrastructure
{
    public class TestStartup
    {
        private readonly IConfiguration _configuration;

        public TestStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SmtpSettings>(_configuration.GetSection("App:Smtp"));
            services.AppAddEmailClient(_configuration);
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Nothing to do here.
        }
    }
}
