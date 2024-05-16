using Enigmatry.Entry.Core.Templating;
using Enigmatry.Entry.TemplatingEngine.Liquid;
using Enigmatry.Entry.TemplatingEngine.Liquid.CustomFilters;
using FluentAssertions;
using Fluid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Enigmatry.Entry.TemplatingEngine.Fluid.Tests;

[Category("unit")]
public class LiquidTemplatingEngineTests
{
    private IServiceScope _scope = null!;

    private enum Foo
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
        _ = services.AddLogging();

        var options = new FluidTemplateEngineOptions
        {
            ConvertEnumToString = true,
            MemberNameStrategy = MemberNameStrategies.SnakeCase,
            CultureInfo = CultureInfo.GetCultureInfo("nl-NL")
        };
        services.AddSingleton(Options.Create(options));
        services.AddLiquidTemplatingEngine();
        _ = services.AddScoped<ICustomFluidFilter, ToCurrencyCustomFluidFilter>();
        _scope = services
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true })
            .CreateScope();
    }

    [Test]
    public async Task EnumShouldBeRenderedAsString()
    {
        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();
        const string template =
            "{% if record.enum_value == \"Bar\" %}BAR{% elsif record.enum_value == \"Baz\" %}BAZ{% elsif record.enum_value == \"Qux\" %}QUX{% endif %}";
        foreach (var value in Enum.GetValues<Foo>())
        {
            var model = new { Record = new { EnumValue = value } };
            var result = await engine.RenderFromFileAsync(template, model);
            _ = result.Should().Be(model.Record.EnumValue.ToString().ToUpperInvariant());
        }
    }

    [Test]
    [TestCase(59250, "to_currency", "59,250.00")]
    [TestCase(59250, "to_currency: '€ ', 2, \".\", \",\" ", "€ 59.250,00")]
    [TestCase(15, "to_currency: '€ ', 2, \".\", \",\" ", "€ 15,00")]
    [TestCase(15, "to_currency: '€ ', 3, \".\", \",\" ", "€ 15,000")]
    [TestCase(null, "to_currency: '€ ', 2, \".\", \",\" ", "")]
    public async Task ToCurrencyShouldReturnFormatedValue(decimal? amount, string customFluidFilter,
        string expectedAmount)
    {
        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();
        var template = "{{ record.amount | " + customFluidFilter + " }}";
        var model = new { Record = new { Amount = amount } };
        var result = await engine.RenderFromFileAsync(template, model);
        _ = result.Should().Be(expectedAmount);
    }

    [Test]
    public async Task IdentifiersShouldBeSnakedCasedInTemplates()
    {
        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();

        var model = new { IdentifierConsistingOfMultipleWords = "hello from template" };

        var snakeCaseResult = await engine.RenderFromFileAsync("{{identifier_consisting_of_multiple_words}}", model);
        _ = snakeCaseResult.Should().Be(model.IdentifierConsistingOfMultipleWords);

        var pascalCaseResult =
            await engine.RenderFromFileAsync($"{{{{{nameof(model.IdentifierConsistingOfMultipleWords)}}}}}", model);
        _ = pascalCaseResult.Should().Be(string.Empty);
    }

    [Test]
    public async Task UnknownIdentifiersShouldBeRenderedAsEmptyString()
    {
        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();

        var model = new { Foo = 42, Bar = "Qux" };

        var result = await engine.RenderFromFileAsync("{{foo}}{{does_not_exist}}{{bar}}", model);
        _ = result.Should().Be(model.Foo + model.Bar);
    }

    [TestCase(null, "{{ date_time }}", "")]
    [TestCase(null, """{{ date_time | date: "%d-%m-%Y" }}""", "")]
    [TestCase("2022-09-14 23:59:59+02:00", "{{ date_time }}", "14-09-2022 23:59:59")]
    [TestCase("2022-09-14 23:59:59+02:00", """{{ date_time | date: "%d-%m-%Y" }}""", "14-09-2022")]
    public async Task TestNullableDateTimeOffsetRendering(string? value, string template, string expected)
    {
        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();

        object model = value == null ? new { DateTime = null as DateTimeOffset? } : new { DateTime = DateTimeOffset.Parse(value) };
        var result = await engine.RenderFromFileAsync(template, model);

        _ = result.Should().Be(expected);
    }

    [TearDown]
    public void TearDown() => _scope.Dispose();
}
