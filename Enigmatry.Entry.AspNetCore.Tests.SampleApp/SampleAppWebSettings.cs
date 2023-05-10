namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp;

public class SampleAppSettings
{
    public bool AuthenticationEnabled { get; set; }
    public bool UseNewtonsoftJson { get; set; } = true;

    public static SampleAppSettings Default() => new SampleAppSettings();
}
