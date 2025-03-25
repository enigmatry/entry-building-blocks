using System.Reflection;
using Shouldly;

namespace Enigmatry.Entry.Scheduler.Tests;

[Category("unit")]
public class TypeExtensionsFixture
{
    [Test]
    public async Task TestFindJobTypes()
    {
        var jobs = ThisAssembly().FindAllJobTypes().ToList();

        jobs.ShouldContain(typeof(SampleJobs.AJob), "this type is implementing IJob interface");
        jobs.ShouldContain(typeof(SampleJobs.AnEntryJobWithArguments), "this type is deriving from EntryJob");
        jobs.ShouldContain(typeof(SampleJobs.AnEntryJobWithoutArguments), "this type is deriving from EntryJob");
        jobs.ShouldContain(typeof(SampleJobs.AJobWithoutCronExpression), "this type is deriving from EntryJob");
        jobs.ShouldContain(typeof(SampleJobs.AJobWithoutConfiguration), "this type is deriving from EntryJob");
        jobs.ShouldContain(typeof(SampleJobs.AnEntryJobDerivingSomeBaseJob), "this type is deriving from a job that derives from EntryJob");
        jobs.ShouldNotContain(typeof(SampleJobs.ABaseNonAbstractJob<>), "this job is a base class");
        jobs.ShouldNotContain(typeof(SampleJobs.ABaseAbstractJob<>), "this job is a base class");

        await Verify(jobs);
    }

    private static Assembly ThisAssembly() => typeof(SampleJobs.AJob).Assembly;
}
