namespace Enigmatry.Entry.AzureSearch.Abstractions;

public interface ISearchIndexManager<T>
{
    public Task<bool> DeleteIndex(CancellationToken cancellationToken = default);
    public Task<bool> RecreateIndex(CancellationToken cancellationToken = default);
}
