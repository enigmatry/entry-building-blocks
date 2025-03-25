using Microsoft.Extensions.Configuration;
using Shouldly;

namespace Enigmatry.Entry.Scheduler.Tests;

[Category("unit")]
public class JobConfigurationFixture
{
    [Test]
    public void GivenJobWithArguments_GetGetSchedulingJobArgumentsValue_Works()
    {
        var arguments =
            GetArguments<SampleJobs.AnEntryJobWithArguments, SampleJobs.AnEntryJobWithArguments.Request>();

        arguments.Arg1.ShouldBe("argument1");
        arguments.Arg2.ShouldBe("argument2");
    }

    [Test]
    public void GivenJobWithoutArguments_GetGetSchedulingJobArgumentsValue_Works()
    {
        var arguments =
            GetArguments<SampleJobs.AnEntryJobWithoutArguments, SampleJobs.AnEntryJobWithoutArguments.Request>();

        arguments.AnArgument.ShouldBe("initialValue");
    }

    private static TArguments GetArguments<T, TArguments>() where TArguments : new()
    {
        var configuration = AConfiguration();
        var jobConfig = configuration.GetJobConfiguration(typeof(T));
        return jobConfig.GetSchedulingJobArgumentsValue<TArguments>();
    }

    private static IConfiguration AConfiguration() => SchedulingConfigurationBuilder.BuildValidTestConfiguration();
}
