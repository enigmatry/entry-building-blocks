using JetBrains.Annotations;
using System;

namespace Enigmatry.BuildingBlocks.Core
{
    [PublicAPI]
    public interface ITimeProvider
    {
        DateTimeOffset Now { get; }
    }
}
