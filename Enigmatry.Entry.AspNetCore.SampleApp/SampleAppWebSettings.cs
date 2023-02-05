namespace Enigmatry.Entry.AspNetCore.SampleApp;

public class SampleAppSettings
{
    public bool UseNewtonsoftJson { get; set; } = true;

    public static SampleAppSettings Default() => new SampleAppSettings();
}
