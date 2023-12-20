using JetBrains.Annotations;
using System;

namespace Enigmatry.Entry.AzureSearch;

[PublicAPI]
public class SearchSettings
{
    public Uri SearchServiceEndPoint { get; set; } = null!;
    public string ApiKey { get; set; } = string.Empty;

    internal void CopyPropertiesTo(SearchSettings target)
    {
        target.SearchServiceEndPoint = SearchServiceEndPoint;
        target.ApiKey = ApiKey;
    }
}
