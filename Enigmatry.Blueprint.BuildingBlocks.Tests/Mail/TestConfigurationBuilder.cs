using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;

namespace Enigmatry.Blueprint.BuildingBlocks.Tests.Infrastructure
{
    public class TestConfigurationBuilder
    {
        public IConfiguration Build()
        {
            var configurationBuilder = new ConfigurationBuilder();

            var dict = new Dictionary<string, string>
            {
                {"App:Smtp:UsePickupDirectory", "true"},
                {"App:Smtp:PickupDirectoryLocation", GetSmtpPickupDirectoryLocation()},
            };

            configurationBuilder.AddInMemoryCollection(dict);
            return configurationBuilder.Build();
        }

        private static string GetSmtpPickupDirectoryLocation() => TestContext.CurrentContext.TestDirectory;
    }
}
