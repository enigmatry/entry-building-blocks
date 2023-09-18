using Azure.Search.Documents;

namespace Enigmatry.Entry.AzureSearch.Abstractions;

public interface ISearchClientFactory<T>
{
    public SearchClient Create();
}
