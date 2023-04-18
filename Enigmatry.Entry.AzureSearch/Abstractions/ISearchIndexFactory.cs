using Azure.Search.Documents.Indexes.Models;

namespace Enigmatry.Entry.AzureSearch.Abstractions;

public interface ISearchIndexFactory<T>
{
    public SearchIndex Create();
}
