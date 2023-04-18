using Azure.Search.Documents.Indexes.Models;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AzureSearch.Tests.Setup;

[UsedImplicitly]
public class TestDocumentSearchIndexFactory : DefaultSearchIndexFactory<TestDocument>
{
    public TestDocumentSearchIndexFactory(ISearchIndexNameResolver<TestDocument> indexNameResolver, ILogger<TestDocumentSearchIndexFactory> logger) : base(indexNameResolver, logger)
    {
    }

    public override SearchIndex Create()
    {
        var index = base.Create();
        index.Analyzers.Add(new LuceneStandardAnalyzer("name"));
        return index;
    }
}
