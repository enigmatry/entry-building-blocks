using System;

namespace Enigmatry.Entry.AzureSearch;

public class SearchSettings
{
    public Uri SearchServiceEndPoint { get; set; } = null!;
    public string ApiKey { get; set; } = String.Empty;

    internal void CopyPropertiesTo(SearchSettings target)
    {
        target.SearchServiceEndPoint = SearchServiceEndPoint;
        target.ApiKey = ApiKey;
    }
}
