using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;

namespace Enigmatry.Entry.Scheduler.Tests;

[Category("unit")]
public class JobEntryFixture
{
    [Test]
    public async Task TestExecute()
    {
        var configuration = SchedulingConfigurationBuilder.BuildValidTestConfiguration();
        var job = new SampleJobs.AnEntryJobWithArguments(new NullLogger<EntryJob<SampleJobs.AnEntryJobWithArguments.Request>>(), configuration);

        await job.Execute(A.Fake<IJobExecutionContext>());

        await Verify(job.ExecutedRequest);
    }
}
