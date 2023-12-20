using JetBrains.Annotations;

namespace Enigmatry.Entry.Scheduler;

[PublicAPI]
public class JobSettings
{
    public bool RunOnStartup { get; set; }
    public bool Enabled { get; set; } = true;
    public string Cronex { get; set; } = string.Empty;
}
