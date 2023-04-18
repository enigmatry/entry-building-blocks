using Azure.Search.Documents.Indexes.Models;

namespace Enigmatry.Entry.AzureSearch;

public interface ISearchIndexFactory<T>
{
    public SearchIndex Create();
}