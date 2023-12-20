namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Http;

public static class UriExtensions
{
    public static Uri AppendParameters(this Uri uri, KeyValuePair<string, string>[] parameters)
    {
        var resourceUri = uri.ToString();
        var filteredParameters = parameters.Where(p => p.Value != null);

        var paramsUri = string.Join("&",
            filteredParameters.Select(p => Uri.EscapeDataString(p.Key) + "=" + Uri.EscapeDataString(p.Value)));

        if (!string.IsNullOrEmpty(paramsUri))
        {
            resourceUri += "?" + paramsUri;
        }

        return new Uri(resourceUri, UriKind.Relative);
    }
}
