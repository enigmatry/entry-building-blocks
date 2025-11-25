using System.Collections.Specialized;
using System.Web;
using Enigmatry.Entry.BlobStorage.Models;

namespace Enigmatry.Entry.BlobStorage.Azure;

internal record AzureBlobSharedUri
{
    private static readonly AzureBlobSharedUri Empty = new();
    private AzureBlobSharedUri() { }

    public string BlobName { get; private set; } = string.Empty;
    public PrivateBlobPermission Permission { get; private set; }
    public DateTimeOffset ExpiresOn { get; private set; }
    public string? CacheControl { get; private set; }
    public string? ContentDisposition { get; private set; }
    public string? ContentEncoding { get; private set; }
    public string? ContentLanguage { get; private set; }
    public string? ContentType { get; private set; }
    public string Signature { get; private set; } = string.Empty;

    public BlobResponseHeadersOverride? GetResponseHeaders() =>
        new(CacheControl, Models.ContentDisposition.Parse(ContentDisposition), ContentEncoding, ContentLanguage, ContentType);

    public static bool TryParse(Uri uri, out AzureBlobSharedUri sharedUri)
    {
        sharedUri = Empty;

        try
        {
            sharedUri = Parse(uri);
        }
        catch (FormatException)
        {
            return false;
        }

        return true;
    }

    private static AzureBlobSharedUri Parse(Uri uri)
    {
        var decodedUriParams = HttpUtility.ParseQueryString(uri.Query);
        var blobName = ParseBlobName(uri.Segments);
        var permission = ParseBlobPermission(decodedUriParams);
        var expiresOn = ParseExpiryDate(decodedUriParams);
        var cacheControl = ParseCacheControl(decodedUriParams);
        var contentDisposition = ParseContentDisposition(decodedUriParams);
        var contentEncoding = ParseContentEncoding(decodedUriParams);
        var contentLanguage = ParseContentLanguage(decodedUriParams);
        var contentType = ParseContentType(decodedUriParams);
        var signature = ParseSignature(decodedUriParams);
        return new AzureBlobSharedUri
        {
            BlobName = blobName,
            ExpiresOn = expiresOn,
            Permission = permission,
            CacheControl = cacheControl,
            ContentDisposition = contentDisposition,
            ContentEncoding = contentEncoding,
            ContentLanguage = contentLanguage,
            ContentType = contentType,
            Signature = signature
        };
    }

    private static string ParseSignature(NameValueCollection decodedUriParams) =>
        decodedUriParams["sig"] ?? throw new FormatException("Cannot parse signature");

    private static string? ParseCacheControl(NameValueCollection decodedUriParams) =>
        decodedUriParams["rscc"];

    private static string? ParseContentDisposition(NameValueCollection decodedUriParams) =>
        decodedUriParams["rscd"];

    private static string? ParseContentEncoding(NameValueCollection decodedUriParams) =>
        decodedUriParams["rsce"];

    private static string? ParseContentLanguage(NameValueCollection decodedUriParams) =>
        decodedUriParams["rscl"];

    private static string? ParseContentType(NameValueCollection decodedUriParams) =>
        decodedUriParams["rsct"];

    private static DateTimeOffset ParseExpiryDate(NameValueCollection decodedUriParams) =>
        DateTimeOffset.TryParse(decodedUriParams["se"], out var expiryDate)
            ? expiryDate
            : throw new FormatException("Cannot parse expiry date");

    private static PrivateBlobPermission ParseBlobPermission(NameValueCollection decodedUriParams) =>
        decodedUriParams["sp"] switch
        {
            "r" => PrivateBlobPermission.Read,
            "w" => PrivateBlobPermission.Write,
            "d" => PrivateBlobPermission.Delete,
            _ => throw new FormatException("Cannot parse permission")
        };

    private static string ParseBlobName(IReadOnlyCollection<string> segments) =>
        segments.Count < 3
            ? throw new FormatException("Cannot parse blob name")
            : segments.Skip(2).Aggregate((acc, seg) => acc + seg);
}
