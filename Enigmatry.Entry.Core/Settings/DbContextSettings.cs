using System;
using JetBrains.Annotations;

namespace Enigmatry.Entry.Core.Settings;

[PublicAPI]
public class DbContextSettings
{
    public bool SensitiveDataLoggingEnabled { get; set; }

    public int ConnectionResiliencyMaxRetryCount { get; set; }

    public TimeSpan ConnectionResiliencyMaxRetryDelay { get; set; }

    public bool RegisterMigrationsAssembly { get; set; }
}
