﻿using System;
using Enigmatry.Blueprint.BuildingBlocks.Core;
using JetBrains.Annotations;

namespace Enigmatry.Blueprint.BuildingBlocks.Infrastructure
{
    [UsedImplicitly]
    public class TimeProvider : ITimeProvider
    {
        private readonly Lazy<DateTimeOffset> _now = new(() => DateTimeOffset.Now);
        public DateTimeOffset Now => _now.Value;
    }
}