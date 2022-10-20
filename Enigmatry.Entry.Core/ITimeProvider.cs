using JetBrains.Annotations;
using System;

namespace Enigmatry.Entry.Core
{
    [PublicAPI]
    public interface ITimeProvider
    {
        DateTimeOffset FixedUtcNow { get; }
        DateTimeOffset UtcNow { get; }
    }
}
