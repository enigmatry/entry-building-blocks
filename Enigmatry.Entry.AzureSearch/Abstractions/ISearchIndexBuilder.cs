using Azure.Search.Documents.Indexes.Models;

namespace Enigmatry.Entry.AzureSearch.Abstractions;

public interface ISearchIndexBuilder<T>
{
    public SearchIndex Build();
}
