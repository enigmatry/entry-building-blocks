﻿using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using Enigmatry.Entry.AzureSearch.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Enigmatry.Entry.AzureSearch.Tests;

[Category("integration")]
public class SearchIndexManagerFixture
{
    private ServiceProvider _services = null!;
    private ISearchIndexManager<TestDocument> _indexManager = null!;

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollectionBuilder().Build();

        _indexManager = _services.GetRequiredService<ISearchIndexManager<TestDocument>>();
    }

    [TearDown]
    public void TearDown() => _services.Dispose();

    [Test]
    public async Task TestRebuildIndex() => await _indexManager.RecreateIndex();

    [Test]
    public async Task TestDeleteIndex()
    {
        // first recreate
        await _indexManager.RecreateIndex();

        var deleted = await _indexManager.DeleteIndex();
        deleted.ShouldBeTrue("index was deleted");

        deleted = await _indexManager.DeleteIndex();
        deleted.ShouldBeFalse("index does not exist since it was deleted");
    }
}
