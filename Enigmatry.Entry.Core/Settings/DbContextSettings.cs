﻿using System;

namespace Enigmatry.Entry.Core.Settings
{
    public class DbContextSettings
    {
        public bool SensitiveDataLoggingEnabled { get; set; }

        public int ConnectionResiliencyMaxRetryCount { get; set; }

        public TimeSpan ConnectionResiliencyMaxRetryDelay { get; set; }
    }
}
