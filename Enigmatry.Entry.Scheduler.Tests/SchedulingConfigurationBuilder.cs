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
#pragma warning disable IDE0055
        // this configuration reflects jobs found in SampleJobs.cs
        var items = new Dictionary<string, string?>
        {
            { "Scheduling:Host", "HostName" },

            { "Scheduling:Jobs:AJob:RunOnStartup", "true" },
            { "Scheduling:Jobs:AJob:Enabled", "true" },
            { "Scheduling:Jobs:AJob:Cronex", "2 0 3 ? * * *" },
            
            { "Scheduling:Jobs:AnEntryJobWithArguments:RunOnStartup", "false" },
            { "Scheduling:Jobs:AnEntryJobWithArguments:Enabled", "false" },
            { "Scheduling:Jobs:AnEntryJobWithArguments:Cronex", "1 0 3 ? * * *" },
            { "Scheduling:Jobs:AnEntryJobWithArguments:Args:Arg1", "argument1" },
            { "Scheduling:Jobs:AnEntryJobWithArguments:Args:Arg2", "argument2" },
            
            { "Scheduling:Jobs:AJobWithoutCronExpression:RunOnStartup", "false" },
            { "Scheduling:Jobs:AJobWithoutCronExpression:Enabled", "false" },
            { "Scheduling:Jobs:AJobWithoutCronExpression:Cronex", "" },

            { "Scheduling:Jobs:AnEntryJobWithoutArguments:RunOnStartup", "false" },
            { "Scheduling:Jobs:AnEntryJobWithoutArguments:Enabled", "false" },
            { "Scheduling:Jobs:AnEntryJobWithoutArguments:Cronex", "1 0 3 ? * * *" }
        };
#pragma warning restore IDE0055

        _configurationBuilder.AddInMemoryCollection(items);
        return _configurationBuilder;
    }

    private IConfiguration Build() => _configurationBuilder.Build();
}
