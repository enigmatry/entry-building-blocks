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
        var job = new SampleJobs.Job2(new NullLogger<EntryJob<SampleJobs.Job2.Request>>(), configuration);

        await job.Execute(A.Fake<IJobExecutionContext>());

        await Verify(job.ExecutedRequest);
    }
}
