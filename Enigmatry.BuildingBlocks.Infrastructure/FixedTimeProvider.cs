using Enigmatry.BuildingBlocks.Core;
using JetBrains.Annotations;
using System;

namespace Enigmatry.BuildingBlocks.Infrastructure
{
    [UsedImplicitly]
    public class FixedTimeProvider : ITimeProvider
    {
        private readonly Lazy<DateTimeOffset> _now = new(() => DateTimeOffset.UtcNow);
        public DateTimeOffset Now => _now.Value;
    }
}
