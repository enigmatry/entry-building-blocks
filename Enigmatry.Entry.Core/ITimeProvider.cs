using System;

namespace Enigmatry.Entry.Core
{
    public interface ITimeProvider
    {
        DateTimeOffset Now { get; }
    }
}
