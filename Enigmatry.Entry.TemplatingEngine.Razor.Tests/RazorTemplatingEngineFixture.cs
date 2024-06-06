using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace Enigmatry.Entry.TemplatingEngine.Tests;

[Category("unit")]
public class RazorTemplatingEngineFixture
{
    private ITemplatingEngine _templatingEngine;

    [SetUp]
    public void Setup()
    {
        IHost host = BuildHost();
        IServiceScopeFactory scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
        IServiceScope serviceScope = scopeFactory.CreateScope();
        _templatingEngine = serviceScope.ServiceProvider.GetRequiredService<ITemplatingEngine>();
    }

    [Test]
    public async Task TestRenderFromFile()
    {
        var result = await _templatingEngine.RenderFromFileAsync("~/Templates/Sample.cshtml",
            new EmailModel { SampleText = "Hello world!" });

        result.Should().Contain("Hello world!");
        result.Should().Contain("Congratulations!");
    }

    [Test]
    public async Task TestRenderFromFileUsingViewBag()
    {
        var viewBagDictionary = new Dictionary<string, object> { ["Title"] = "Hello world!" };

        var result = await _templatingEngine.RenderFromFileAsync("~/Templates/SampleWithViewBag.cshtml",
            new EmailModel { SampleText = "Congratulations!" }, viewBagDictionary);

        result.Should().Contain("Hello world!");
        result.Should().Contain("Congratulations!");
    }

    private static IHost BuildHost() =>
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<RazorSampleConsoleStartup>();
            }).Build();
}
