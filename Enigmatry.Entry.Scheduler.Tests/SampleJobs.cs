using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Enigmatry.Entry.Scheduler.Tests;

public static class SampleJobs
{
    [UsedImplicitly]
    internal class AJob : IJob
    {
        public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
    }

    [UsedImplicitly]
    internal class AnEntryJobWithArguments : EntryJob<AnEntryJobWithArguments.Request>
    {
        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        internal class Request
        {
            public string Arg1 { get; set; } = string.Empty;
            public string Arg2 { get; set; } = string.Empty;
        }

        public AnEntryJobWithArguments(ILogger<EntryJob<Request>> logger, IConfiguration configuration) : base(logger, configuration)
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
    internal class AJobWithoutCronExpression : IJob
    {
        public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
    }

    [UsedImplicitly]
    internal class AJobWithoutConfiguration : IJob
    {
        public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
    }

    [UsedImplicitly]
    public class AnEntryJobWithoutArguments : EntryJob<AnEntryJobWithoutArguments.Request>
    {
        public AnEntryJobWithoutArguments(ILogger<EntryJob<Request>> logger, IConfiguration configuration) : base(logger,
            configuration)
        {
        }

        public override Task Execute(Request request) => Task.CompletedTask;

        public class Request
        {
            public string AnArgument { get; set; } = "initialValue";
        }
    }

    [UsedImplicitly]
    public class AnEntryJobDerivingSomeBaseJob : ABaseNonAbstractJob<AnEntryJobDerivingSomeBaseJob.Request>
    {
        public class Request
        {
        }

        public AnEntryJobDerivingSomeBaseJob(ILogger<EntryJob<Request>> logger, IConfiguration configuration) : base(
            logger,
            configuration)
        {
        }

        public override async Task Execute(Request request) => await base.Execute(request);
    }

    public class ABaseNonAbstractJob<T> : ABaseAbstractJob<T> where T : class, new()
    {
        protected ABaseNonAbstractJob(ILogger<EntryJob<T>> logger, IConfiguration configuration) : base(logger, configuration)
        {
        }

        public override Task Execute(T request) => Task.CompletedTask;
    }

    public abstract class ABaseAbstractJob<T> : EntryJob<T> where T : class, new()
    {
        protected ABaseAbstractJob(ILogger<EntryJob<T>> logger, IConfiguration configuration) : base(logger, configuration)
        {
        }
    }
}
