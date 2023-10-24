using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Setup;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch.Tests;

[Category("unit")]
public class SearchIndexFactoryResolvingFixture
{
    private ServiceProvider _services = null!;

    [SetUp]
    public void Setup() => _services = new ServiceCollectionBuilder().Build();

    [TearDown]
    public void TearDown() => _services.Dispose();

    [Test]
    public void TestResolveSearchIndexTestDocument()
    {
        var resolver = _services.GetRequiredService<ISearchIndexFactory<TestDocument>>();

        var index = resolver.Create();

        index.Name.Should().Be("test-document");
        index.Analyzers.Count.Should().Be(1);
    }

    [Test]
    public void TestResolveSearchIndexAnotherTestDocument()
    {
        var resolver = _services.GetRequiredService<ISearchIndexFactory<AnotherTestDocument>>();

        var index = resolver.Create();

        index.Name.Should().Be("another-test-document");
        index.Analyzers.Count.Should().Be(0);
    }
}
