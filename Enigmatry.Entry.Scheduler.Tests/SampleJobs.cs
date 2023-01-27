using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Enigmatry.Entry.Scheduler.Tests;

public static class SampleJobs
{
    [UsedImplicitly]
    internal class Job1 : IJob
    {
        public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
    }

    [UsedImplicitly]
    internal class Job2 : EntryJob<Job2.Request>
    {
        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        internal class Request
        {
            public string Arg1 { get; set; } = String.Empty;
            public string Arg2 { get; set; } = String.Empty;
        }

        public Job2(ILogger<EntryJob<Request>> logger, IConfiguration configuration) : base(logger, configuration)
        {
        }

        public override Task Execute(Request request)
        {
            ExecutedRequest = request;// remember the executed request so that we can check it
            return Task.CompletedTask;
        }

        internal Request? ExecutedRequest { get; private set; }
    }

    [UsedImplicitly]
    internal class Job3 : IJob
    {
        public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
    }

    [UsedImplicitly]
    internal class Job4 : IJob
    {
        public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
    }

    [UsedImplicitly]
    public class AJob : IJob
    {
        public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
    }

    [UsedImplicitly]
    public class AnEntryJob : EntryJob<AnEntryJob.Request>
    {
        public AnEntryJob(ILogger<EntryJob<Request>> logger, IConfiguration configuration) : base(logger,
            configuration)
        {
        }

        public override Task Execute(Request request) => Task.CompletedTask;

        public class Request
        {
        }
    }

    [UsedImplicitly]
    public class AnEntryJobDerivingSomeBaseJob : ABaseJob<AnEntryJobDerivingSomeBaseJob.Request>
    {
        public class Request
        {
        }

        public AnEntryJobDerivingSomeBaseJob(ILogger<EntryJob<Request>> logger, IConfiguration configuration) : base(
            logger,
            configuration)
        {
        }

        public override Task Execute(Request request) => throw new NotImplementedException();
    }

    public abstract class ABaseJob<T> : EntryJob<T> where T : class, new()
    {
        protected ABaseJob(ILogger<EntryJob<T>> logger, IConfiguration configuration) : base(logger, configuration)
        {
        }
    }
}
