using Enigmatry.Entry.Core.Helpers;
using Enigmatry.Entry.TemplatingEngine.Liquid;
using Enigmatry.Entry.TemplatingEngine.Liquid.CustomFilters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.TemplatingEngine.Fluid.Tests;

[Category("unit")]
public class LiquidTemplatingEngineTests
{
    private IServiceScope _scope = null!;

    public enum Foo
    {
        [System.ComponentModel.Description("Bar")]
        Bar,

        [System.ComponentModel.Description("Baz")]
        Baz,

        [System.ComponentModel.Description("Qux")]
        Qux
    }

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLiquidTemplatingEngine(options => options.ConvertEnumToString = true);
        services.AddScoped<ICustomFluidFilter, ToCurrencyCustomFluidFilter>();
        _scope = services
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true })
            .CreateScope();
    }

    [Test]
    public async Task EnumShouldBeRenderedAsString()
    {
        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();
        var template = "{% if testValue == \"Bar\" %}BAR{% elsif testValue == \"Baz\" %}BAZ{% elsif testValue == \"Qux\" %}QUX{% endif %}";

        foreach (var value in Enum.GetValues<Foo>())
        {
            var model = new { TestValue = value };
            var result = await engine.RenderFromFileAsync(template, model);
            _ = result.Should().Be(value.GetDescription().ToUpperInvariant());
        }
    }

    [Test]
    [TestCase(59250, "to_currency", "59,250.00")]
    [TestCase(59250, "to_currency: '€ ', 2, \".\", \",\" ", "€ 59.250,00")]
    [TestCase(15, "to_currency: '€ ', 2, \".\", \",\" ", "€ 15,00")]
    [TestCase(15, "to_currency: '€ ', 3, \".\", \",\" ", "€ 15,000")]
    [TestCase(null, "to_currency: '€ ', 2, \".\", \",\" ", "")]
    public async Task ToCurrencyShouldReturnFormatedValue(decimal? amount, string customFluidFilter, string expectedAmount)
    {
        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();
        var template = "{{ record.amount | " + customFluidFilter + " }}";
        var model = new { Record = new { Amount = amount } };
        var result = await engine.RenderFromFileAsync(template, model);
        _ = result.Should().Be(expectedAmount);
    }

    [TearDown]
    public void TearDown() => _scope.Dispose();
}
