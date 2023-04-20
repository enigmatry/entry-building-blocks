namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp;

public class SampleAppSettings
{
    public bool UseNewtonsoftJson { get; set; } = true;

    public static SampleAppSettings Default() => new();
}
