using System.Threading;
using System.Threading.Tasks;
using Azure.Search.Documents.Indexes;

namespace Enigmatry.Entry.AzureSearch;

public class DefaultSearchIndexManager<T> : ISearchIndexManager<T>
{
    private readonly ISearchIndexFactory<T> _indexFactory;
    private readonly SearchIndexClient _client;
    private const int NoContentHttpStatusCode = 204;

    public DefaultSearchIndexManager(ISearchIndexFactory<T> indexFactory, SearchIndexClient client)
    {
        _indexFactory = indexFactory;
        _client = client;
    }

    public async Task<bool> DeleteIndex(CancellationToken cancellationToken = default)
    {
        var index = _indexFactory.Create();
        var response = await _client.DeleteIndexAsync(index, false, cancellationToken);

        return response.Status == NoContentHttpStatusCode;
    }

    public async Task<bool> RecreateIndex(CancellationToken cancellationToken)
    {
        var index = _indexFactory.Create();

        await _client.DeleteIndexAsync(index, false, cancellationToken);
        await _client.CreateIndexAsync(index, cancellationToken);
        return true;
    }
}
