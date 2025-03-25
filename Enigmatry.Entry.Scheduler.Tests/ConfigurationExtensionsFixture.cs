using System.Configuration;
using Microsoft.Extensions.Configuration;
using Shouldly;

namespace Enigmatry.Entry.Scheduler.Tests;

[Category("unit")]
public class ConfigurationExtensionsFixture
{
    [Test]
    public void GivenNoTypes_FindAllJobConfigurations_ReturnsNothing()
    {
        var result = AConfigurationWithoutSchedulingSection().FindAllJobConfigurations(Enumerable.Empty<Type>());

        result.ShouldBeEmpty();
    }

    [Test]
    public void GivenConfigurationWithoutSchedulingSection_FindAllJobConfigurations_ThrowsError()
    {
        var act = () =>
        {
            _ = AConfigurationWithoutSchedulingSection().FindAllJobConfigurations(JobTypesWithConfiguration()).ToList();
        };

        var exception = Should.Throw<ConfigurationErrorsException>(act);
        exception.Message.ShouldBe("Section 'Scheduling' is missing from the configuration");
    }

    [Test]
    public void GivenWrongJobTypes_FindAllJobConfigurations_ThrowsError()
    {
        var act = () =>
        {
            _ = AConfiguration().FindAllJobConfigurations(WrongJobTypes()).ToList();
        };

        var exception = Should.Throw<ConfigurationErrorsException>(act);
        exception.Message.ShouldBe($"Configuration Section '{WrongJobTypes().First().Name}' is not found");
    }

    [Test]
    public void GivenJobTypesWithoutValidConfiguration_FindAllJobConfigurations_ThrowsError()
    {
        var act = () =>
        {
            _ = AConfiguration().FindAllJobConfigurations(JobTypesWithoutValidConfiguration()).ToList();
        };

        var exception = Should.Throw<ConfigurationErrorsException>(act);
        exception.Message.ShouldBe($"Missing 'Cronex' value in configuration for configuration section: '{JobTypesWithoutValidConfiguration().First().Name}'");
    }

    [Test]
    public void GivenJobTypesWithoutConfiguration_FindAllJobConfigurations_ThrowsError()
    {
        var act = () =>
        {
            _ = AConfiguration().FindAllJobConfigurations(JobTypesWithoutConfiguration()).ToList();
        };

        var exception = Should.Throw<ConfigurationErrorsException>(act);
        exception.Message.ShouldBe($"Configuration Section '{JobTypesWithoutConfiguration().First().Name}' is not found");
    }

    [Test]
    public async Task GivenJobTypesWithConfiguration_FindAllJobConfigurations_Works()
    {
        var jobs = AConfiguration().FindAllJobConfigurations(JobTypesWithConfiguration()).ToList();
        jobs.Count.ShouldBe(2);
        await Verify(jobs);

        var job2 = jobs.First(j => j.JobType == typeof(SampleJobs.AnEntryJobWithArguments));
        var args = job2.GetSchedulingJobArgumentsValue<SampleJobs.AnEntryJobWithArguments.Request>();
        args.Arg1.ShouldBe("argument1");
        args.Arg2.ShouldBe("argument2");
    }

    private static IConfiguration AConfigurationWithoutSchedulingSection() =>
        SchedulingConfigurationBuilder.BuildConfigurationWithoutSchedulingSection();

    private static IConfiguration AConfiguration() => SchedulingConfigurationBuilder.BuildValidTestConfiguration();

    private static IEnumerable<Type> JobTypesWithConfiguration() =>
        new List<Type> { typeof(SampleJobs.AJob), typeof(SampleJobs.AnEntryJobWithArguments) };

    private static IEnumerable<Type> WrongJobTypes() =>
        new List<Type> { typeof(ConfigurationExtensionsFixture), typeof(int) };

    private static IEnumerable<Type> JobTypesWithoutValidConfiguration() =>
        new List<Type> { typeof(SampleJobs.AJobWithoutCronExpression) };

    private static IEnumerable<Type> JobTypesWithoutConfiguration() =>
        new List<Type> { typeof(SampleJobs.AJobWithoutConfiguration) };
}
