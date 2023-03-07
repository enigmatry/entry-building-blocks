using JetBrains.Annotations;
using System;

namespace Enigmatry.Entry.Core
{
    /// <summary>
    /// Represents abstraction over time management. Makes any time-based code easily testable.
    /// </summary>
    [PublicAPI]
    public interface ITimeProvider
    {
        /// <summary>
        /// Returns current UTC time for the first invocation. Subsequent invocations return cached value (the same value as in first invocation).
        /// </summary>
        DateTimeOffset FixedUtcNow { get; }

        /// <summary>
        /// Always returns current UTC time, no matter how many times you invoke it.
        /// </summary>
        DateTimeOffset UtcNow { get; }
    }
}
