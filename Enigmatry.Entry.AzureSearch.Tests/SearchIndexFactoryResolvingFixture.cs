using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

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
        var resolver = _services.GetRequiredService<ISearchIndexBuilder<TestDocument>>();

        var index = resolver.Build();

        index.Name.ShouldBe("test-document");
        index.Analyzers.Count.ShouldBe(1);
    }

    [Test]
    public void TestResolveSearchIndexAnotherTestDocument()
    {
        var resolver = _services.GetRequiredService<ISearchIndexBuilder<AnotherTestDocument>>();

        var index = resolver.Build();

        index.Name.ShouldBe("another-test-document");
        index.Analyzers.Count.ShouldBe(0);
    }
}
