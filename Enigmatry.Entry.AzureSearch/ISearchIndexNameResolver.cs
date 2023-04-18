namespace Enigmatry.Entry.AzureSearch;

public interface ISearchIndexNameResolver<T>
{
    public string ResolveIndexName();
}
