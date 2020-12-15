using Enigmatry.Blueprint.BuildingBlocks.Email;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Blueprint.BuildingBlocks.Tests.Mail
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
            services.AppAddEmailClient(_configuration);
        }

        [UsedImplicitly]
#pragma warning disable CA1801 // Review unused parameters
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore CA1801 // Review unused parameters
        {
            // Nothing to do here.
        }
    }
}
