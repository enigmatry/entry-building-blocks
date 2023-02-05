using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.Entry.AspNetCore.TestUtils.Http;

public static class UriExtensions
{
    public static Uri AppendParameters(this Uri uri, KeyValuePair<string, string>[] parameters)
    {
        var resourceUri = uri.ToString();
        var filteredParameters = parameters.Where(p => p.Value != null);

        var paramsUri = String.Join("&",
            filteredParameters.Select(p => Uri.EscapeDataString(p.Key) + "=" + Uri.EscapeDataString(p.Value)));

        if (!String.IsNullOrEmpty(paramsUri))
        {
            resourceUri += "?" + paramsUri;
        }

        return new Uri(resourceUri, UriKind.Relative);
    }
}
