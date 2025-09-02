using Azure.Search.Documents.Indexes.Models;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Enigmatry.Entry.AzureSearch.Indexes;
using Enigmatry.Entry.AzureSearch.Tests.Documents;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AzureSearch.Tests.Setup;

[UsedImplicitly]
public class TestDocumentSearchIndexFactory : DefaultSearchIndexBuilder<TestDocument>
{
    public TestDocumentSearchIndexFactory(ISearchIndexNameResolver<TestDocument> indexNameResolver, ILogger<TestDocumentSearchIndexFactory> logger) : base(indexNameResolver, logger)
    {
    }

    public override SearchIndex Build()
    {
        var index = base.Build();
        index.Analyzers.Add(new LuceneStandardAnalyzer("name"));
        return index;
    }
}
