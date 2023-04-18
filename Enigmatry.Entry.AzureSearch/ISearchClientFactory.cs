using Azure.Search.Documents;

namespace Enigmatry.Entry.AzureSearch;

public interface ISearchClientFactory<T>
{
    public SearchClient Create();
}