using Enigmatry.Entry.TemplatingEngine.Liquid;
using FluentAssertions;
using Fluid;
using Microsoft.Extensions.DependencyInjection;
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

        services.AddLiquidTemplatingEngine(options =>
        {
            options.MemberNameStrategy = MemberNameStrategies.SnakeCase;
            options.CultureInfo = CultureInfo.GetCultureInfo("nl-NL");
            options.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");
        });

        _scope = services
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true })
            .CreateScope();
    }

    [Test]
    public async Task IdentifiersShouldBeSnakedCasedInTemplates()
    {
        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();

        var model = new { IdentifierConsistingOfMultipleWords = "hello from template" };

        var snakeCaseResult = await engine.RenderAsync("{{identifier_consisting_of_multiple_words}}", model);
        _ = snakeCaseResult.Should().Be(model.IdentifierConsistingOfMultipleWords);

        var pascalCaseResult =
            await engine.RenderAsync($"{{{{{nameof(model.IdentifierConsistingOfMultipleWords)}}}}}", model);
        _ = pascalCaseResult.Should().Be(string.Empty);
    }

    [Test]
    public async Task UnknownIdentifiersShouldBeRenderedAsEmptyString()
    {
        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();

        var model = new { Foo = 42, Bar = "Qux" };

        var result = await engine.RenderAsync("{{foo}}{{does_not_exist}}{{bar}}", model);
        _ = result.Should().Be(model.Foo + model.Bar);
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
            var result = await engine.RenderAsync(template, model);
            _ = result.Should().Be(model.Record.EnumValue.ToString().ToUpperInvariant());
        }
    }

    [Test]
    [TestCase(59250, "format_number: 'n2'", "59.250,00")]
    [TestCase(59250, "format_number: 'n2', 'en-US'", "59,250.00")]
    [TestCase(59250, "format_number: 'c2'", "€ 59.250,00")]
    [TestCase(15, "format_number: 'c2'", "€ 15,00")]
    [TestCase(15, "format_number: 'c3'", "€ 15,000")]
    [TestCase(null, "format_number: 'n2'", "")]
    public async Task NumbersShouldBeCorrectlyFormatted(decimal? amount, string fluidFilter, string expectedAmount)
    {
        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();
        var template = "{{ record.amount | " + fluidFilter + " }}";
        var model = new { Record = new { Amount = amount } };
        var result = await engine.RenderAsync(template, model);
        _ = result.Should().Be(expectedAmount);
    }

    [TestCase(null, "{{ date_time }}", "")]
    [TestCase("2022-09-14 23:59:59+02:00", "{{ date_time }}", "14-09-2022 23:59:59")]
    [TestCase("2022-09-14 23:59:59+00:00", "{{ date_time }}", "15-09-2022 01:59:59")]
    [TestCase("2022-09-14 23:59:59+00:00", """{{ date_time | date: "%Y-%m-%d" }}""", "2022-09-15")]
    public async Task DateTimeOffsetShouldBeCorrectlyFormatted(string? value, string template, string expected)
    {
        object model = value == null
            ? new { DateTime = null as DateTimeOffset? }
            : new { DateTime = DateTimeOffset.Parse(value) };

        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();
        var result = await engine.RenderAsync(template, model);

        _ = result.Should().Be(expected);
    }

    [TestCase(null, "{{ date_time }}", "")]
    [TestCase("2022-09-14 23:59:59Z", "{{ date_time }}", "15-09-2022 01:59:59")]
    [TestCase("2022-09-14 23:59:59Z", """{{ date_time | date: "%d-%m-%Y" }}""", "15-09-2022")]
    public async Task DateTimeShouldBeCorrectlyFormatted(string? value, string template, string expected)
    {
        object model = value == null
            ? new { DateTime = null as DateTime? }
            : new { DateTime = DateTime.Parse(value) };

        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();
        var result = await engine.RenderAsync(template, model);

        _ = result.Should().Be(expected);
    }

    [TestCase(null, "{{ date_only }}", "")]
    [TestCase("2022-09-14", "{{ date_only }}", "14-09-2022")]
    public async Task DateOnlyShouldBeCorrectlyFormatted(string? value, string template, string expected)
    {
        object model = value == null
            ? new { DateOnly = null as DateOnly? }
            : new { DateOnly = DateOnly.Parse(value) };

        var engine = _scope.ServiceProvider.GetRequiredService<ITemplatingEngine>();
        var result = await engine.RenderAsync(template, model);

        _ = result.Should().Be(expected);
    }

    [TearDown]
    public void TearDown() => _scope.Dispose();
}
