using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Enigmatry.Entry.Scheduler.Tests;

[Category("unit")]
public class JobConfigurationFixture
{
    [Test]
    public void GivenJobWithArguments_GetGetSchedulingJobArgumentsValue_Works()
    {
        var arguments =
            GetArguments<SampleJobs.AnEntryJobWithArguments, SampleJobs.AnEntryJobWithArguments.Request>();

        arguments.Arg1.Should().Be("argument1");
        arguments.Arg2.Should().Be("argument2");
    }

    [Test]
    public void GivenJobWithoutArguments_GetGetSchedulingJobArgumentsValue_Works()
    {
        var arguments =
            GetArguments<SampleJobs.AnEntryJobWithoutArguments, SampleJobs.AnEntryJobWithoutArguments.Request>();

        arguments.AnArgument.Should().Be("initialValue");
    }

    private static TArguments GetArguments<T, TArguments>() where TArguments : new()
    {
        var configuration = AConfiguration();
        var jobConfig = configuration.GetJobConfiguration(typeof(T));
        return jobConfig.GetSchedulingJobArgumentsValue<TArguments>();
    }

    private static IConfiguration AConfiguration() => SchedulingConfigurationBuilder.BuildValidTestConfiguration();
}
