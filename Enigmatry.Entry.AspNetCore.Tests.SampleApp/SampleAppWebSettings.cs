namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp;

public class SampleAppSettings
{
    public bool IsUserAuthenticated { get; set; } = true;
    public bool UseNewtonsoftJson { get; set; }

    public static SampleAppSettings Default() => new();
}
