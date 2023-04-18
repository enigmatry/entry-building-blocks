using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Enigmatry.Entry.Email.Tests.Infrastructure;

public static class TestConfigurationBuilder
{
    public static IConfiguration Build()
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
