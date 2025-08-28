using Enigmatry.Entry.AzureSearch.Abstractions;
using Humanizer;

namespace Enigmatry.Entry.AzureSearch.Indexes;

public class DefaultSearchIndexNameResolver<T> : ISearchIndexNameResolver<T>
{
    private readonly string _indexName = typeof(T).Name.Kebaberize();

    public DefaultSearchIndexNameResolver()
    {
    }

    public DefaultSearchIndexNameResolver(string indexName)
    {
        _indexName = indexName;
    }

    public string ResolveIndexName() => _indexName;
}
