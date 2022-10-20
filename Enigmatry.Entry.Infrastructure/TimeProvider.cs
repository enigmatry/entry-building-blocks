using Enigmatry.Entry.Core;
using System;

namespace Enigmatry.Entry.Infrastructure
{
    public class TimeProvider : ITimeProvider
    {
        private readonly Lazy<DateTimeOffset> _now = new(() => DateTimeOffset.UtcNow);
        public DateTimeOffset FixedUtcNow => _now.Value;
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
