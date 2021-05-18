using System;
using Enigmatry.BuildingBlocks.Core;
using JetBrains.Annotations;

namespace Enigmatry.BuildingBlocks.Infrastructure
{
    [UsedImplicitly]
    public class TimeProvider : ITimeProvider
    {
        private readonly Lazy<DateTimeOffset> _now = new(() => DateTimeOffset.UtcNow);
        public DateTimeOffset Now => _now.Value;
    }
}
