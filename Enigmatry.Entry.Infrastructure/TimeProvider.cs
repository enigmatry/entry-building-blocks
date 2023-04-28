using Enigmatry.Entry.Core;
using Enigmatry.Entry.Core.Times;
using JetBrains.Annotations;
using System;

namespace Enigmatry.Entry.Infrastructure
{
    [PublicAPI]
    public class TimeProvider : ITimeProvider
    {
        private readonly Lazy<DateTimeOffset> _now = new(() => DateTimeOffset.UtcNow);

        public DateTimeOffset FixedUtcNow => _now.Value;
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        public bool InFuture(Period period) => period.StartDate > UtcNow;
        public bool InPast(Period period) => period.EndDate < UtcNow;
        public bool InPresent(Period period) => period.Contains(UtcNow);
    }
}
