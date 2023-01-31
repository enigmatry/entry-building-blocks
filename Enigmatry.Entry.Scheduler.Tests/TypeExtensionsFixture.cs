using System.Reflection;
using FluentAssertions;

namespace Enigmatry.Entry.Scheduler.Tests;

[Category("unit")]
public class TypeExtensionsFixture
{
    [Test]
    public async Task TestFindJobTypes()
    {
        var jobs = ThisAssembly().FinAllJobTypes().ToList();

        jobs.Should().Contain(typeof(SampleJobs.AJob), "this type is implementing IJob interface");
        jobs.Should().Contain(typeof(SampleJobs.AnEntryJobWithArguments), "this type is deriving from EntryJob");
        jobs.Should().Contain(typeof(SampleJobs.AnEntryJobWithoutArguments), "this type is deriving from EntryJob");
        jobs.Should().Contain(typeof(SampleJobs.AJobWithoutCronExpression), "this type is deriving from EntryJob");
        jobs.Should().Contain(typeof(SampleJobs.AJobWithoutConfiguration), "this type is deriving from EntryJob");
        jobs.Should().Contain(typeof(SampleJobs.AnEntryJobDerivingSomeBaseJob), "this type is deriving from a job that derives from EntryJob");
        jobs.Should().NotContain(typeof(SampleJobs.ABaseNonAbstractJob<>), "this job is a base class");
        jobs.Should().NotContain(typeof(SampleJobs.ABaseAbstractJob<>), "this job is a base class");

        await Verify(jobs);
    }

    private static Assembly ThisAssembly() => typeof(SampleJobs.AJob).Assembly;
}
