using Azure.Search.Documents.Indexes;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AzureSearch.Indexes;

public class DefaultSearchIndexManager<T> : ISearchIndexManager<T>
{
    private readonly ISearchIndexBuilder<T> _indexBuilder;
    private readonly SearchIndexClient _client;
    private readonly ILogger<DefaultSearchIndexManager<T>> _logger;
    private const int NoContentHttpStatusCode = 204;

    public DefaultSearchIndexManager(ISearchIndexBuilder<T> indexBuilder, SearchIndexClient client,
        ILogger<DefaultSearchIndexManager<T>> logger)
    {
        _indexBuilder = indexBuilder;
        _client = client;
        _logger = logger;
    }

    public async Task<bool> DeleteIndex(CancellationToken cancellationToken = default)
    {
        var index = _indexBuilder.Build();

        _logger.LogInformation("Deleting index: {IndexName}", index.Name);

        var response = await _client.DeleteIndexAsync(index, false, cancellationToken);
        _logger.LogDebug("Response status: {Status}", response.Status);

        var result = response.Status == NoContentHttpStatusCode;
        return result;
    }

    public async Task<bool> RecreateIndex(CancellationToken cancellationToken)
    {
        var index = _indexBuilder.Build();

        _logger.LogInformation("Delete previous index. Index name: {IndexName}", index.Name);

        var deleteResponse = await _client.DeleteIndexAsync(index, false, cancellationToken);
        _logger.LogDebug("Delete index response status: {Status}", deleteResponse.Status);

        var response = await _client.CreateIndexAsync(index, cancellationToken);
        _logger.LogDebug("Create index response status: {Status}", response.GetRawResponse().Status);

        return true;
    }
}
