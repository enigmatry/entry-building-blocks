using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Enigmatry.Entry.AzureSearch.Indexes;

public class DefaultSearchIndexManager<T> : ISearchIndexManager<T>
{
    private readonly ISearchIndexBuilder<T> _indexBuilder;
    private readonly SearchIndexClient _client;
    private readonly ILogger<DefaultSearchIndexManager<T>> _logger;

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

        return await TryDeleteIndex(index, cancellationToken);
    }

    public async Task<bool> RecreateIndex(CancellationToken cancellationToken)
    {
        var index = _indexBuilder.Build();

        _logger.LogInformation("Delete previous index. Index name: {IndexName}", index.Name);

        await TryDeleteIndex(index, cancellationToken);

        var response = await _client.CreateIndexAsync(index, cancellationToken);
        _logger.LogDebug("Create index response status: {Status}", response.GetRawResponse().Status);

        return true;
    }

    private async Task<bool> TryDeleteIndex(SearchIndex index, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _client.DeleteIndexAsync(index, false, cancellationToken);
            _logger.LogDebug("Delete index response status: {Status}", response.Status);

            return response.Status == (int)HttpStatusCode.NoContent;
        }
        catch (RequestFailedException e) when (e.Status == (int)HttpStatusCode.NotFound)
        {
            _logger.LogDebug("Index does not exist: {IndexName}", index.Name);
            return false;
        }
    }
}
