using Microsoft.Extensions.Configuration;

namespace Enigmatry.Entry.Scheduler.Tests;

public class SchedulingConfigurationBuilder
{
    private readonly ConfigurationBuilder _configurationBuilder = new();

    public static IConfiguration BuildValidTestConfiguration() =>
        new SchedulingConfigurationBuilder().WithValidTestConfiguration().Build();

    public static IConfiguration BuildConfigurationWithoutSchedulingSection() => new SchedulingConfigurationBuilder().Build();

    private ConfigurationBuilder WithValidTestConfiguration()
    {
        // this configuration reflects jobs found SampleJobs.cs
        var items = new Dictionary<string, string>
        {
            { "Scheduling:Host", "HostName" },
            { "Scheduling:Jobs:Job1:RunOnStartup", "true" },
            { "Scheduling:Jobs:Job1:Enabled", "true" },
            { "Scheduling:Jobs:Job1:Cronex", "12:00:00" },
            { "Scheduling:Jobs:Job2:RunOnStartup", "false" },
            { "Scheduling:Jobs:Job2:Enabled", "false" },
            { "Scheduling:Jobs:Job2:Cronex", "12:00:00" },
            { "Scheduling:Jobs:Job2:Args:Arg1", "argument1" },
            { "Scheduling:Jobs:Job2:Args:Arg2", "argument2" },
            { "Scheduling:Jobs:Job3:RunOnStartup", "false" },
            { "Scheduling:Jobs:Job3:Enabled", "false" },
            { "Scheduling:Jobs:Job3:Cronex", "" }
        };

        _configurationBuilder.AddInMemoryCollection(items);
        return _configurationBuilder;
    }

    private IConfiguration Build() => _configurationBuilder.Build();
}
