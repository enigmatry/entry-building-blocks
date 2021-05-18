using System;

namespace Enigmatry.BuildingBlocks.Core
{
    public interface ITimeProvider
    {
        DateTimeOffset Now { get; }
    }
}
