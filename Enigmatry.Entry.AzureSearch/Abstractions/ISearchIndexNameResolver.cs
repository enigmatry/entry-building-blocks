namespace Enigmatry.Entry.AzureSearch.Abstractions;

public interface ISearchIndexNameResolver<T>
{
    public string ResolveIndexName();
}
