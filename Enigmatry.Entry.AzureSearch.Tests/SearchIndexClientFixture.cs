using Azure.Search.Documents.Indexes;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AzureSearch.Tests;

[Category("integration")]
public class SearchIndexClientFixture
{
    private ServiceProvider _services = null!;
    private SearchIndexClient _searchIndexClient = null!;
    private ISearchIndexFactory<TestDocument> _indexFactory = null!;

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollectionBuilder().Build();

        _searchIndexClient = _services.GetRequiredService<SearchIndexClient>();
        _indexFactory = _services.GetRequiredService<ISearchIndexFactory<TestDocument>>();
    }

    [TearDown]
    public void TearDown() => _services.Dispose();

    [Test]
    public async Task TestCreateOrUpdateIndex()
    {
        var index = _indexFactory.Create();
        await _searchIndexClient.CreateOrUpdateIndexAsync(index);
    }

    [Test]
    public async Task TestDeleteIndex()
    {
        var index = _indexFactory.Create();
        await _searchIndexClient.CreateOrUpdateIndexAsync(index);
        await _searchIndexClient.DeleteIndexAsync(index.Name);
    }
}
