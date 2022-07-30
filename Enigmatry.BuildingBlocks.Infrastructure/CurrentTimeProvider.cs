using Enigmatry.BuildingBlocks.Core;
using JetBrains.Annotations;
using System;

namespace Enigmatry.BuildingBlocks.Infrastructure
{
    [UsedImplicitly]
    public class CurrentTimeProvider : ITimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}
