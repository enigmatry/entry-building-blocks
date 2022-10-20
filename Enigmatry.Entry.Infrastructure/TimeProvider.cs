using Enigmatry.Entry.Core;
using JetBrains.Annotations;
using System;

namespace Enigmatry.Entry.Infrastructure
{
    /// <summary>
    /// Class represents implemented abstraction over time management. Makes any time-based code easily testable.
    /// </summary>
    [PublicAPI]
    public class TimeProvider : ITimeProvider
    {
        private readonly Lazy<DateTimeOffset> _now = new(() => DateTimeOffset.UtcNow);

        /// <summary>
        /// Returns current UTC time for the first invocation. Subsequent invocations return cached value (the same value as in first invocation).
        /// </summary>
        public DateTimeOffset FixedUtcNow => _now.Value;

        /// <summary>
        /// Always returns current UTC time, no matter how many times you invoke it.
        /// </summary>
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
