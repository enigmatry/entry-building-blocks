using System;

namespace Enigmatry.Blueprint.BuildingBlocks.Core
{
    public interface ITimeProvider
    {
        DateTimeOffset Now { get; }
    }
}
