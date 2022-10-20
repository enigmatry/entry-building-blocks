using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Enigmatry.Entry.BlobStorage.Azure
{
    internal record AzureBlobSharedUri
    {
        private static readonly AzureBlobSharedUri Empty = new();
        private AzureBlobSharedUri() { }

        public string BlobName { get; private set; } = String.Empty;
        public PrivateBlobPermission Permission { get; private set; }
        public DateTimeOffset ExpiresOn { get; private set; }
        public string Signature { get; set; } = String.Empty;

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
            var decodedUri = new Uri(HttpUtility.UrlDecode(uri.AbsoluteUri));
            var decodedUriParams = HttpUtility.ParseQueryString(decodedUri.Query);
            var blobName = ParseBlobName(decodedUri.Segments);
            var permission = ParseBlobPermission(decodedUriParams);
            var expiresOn = ParseExpiryDate(decodedUriParams);
            var signature = ParseSignature(decodedUriParams);
            return new AzureBlobSharedUri
            {
                BlobName = blobName,
                ExpiresOn = expiresOn,
                Permission = permission,
                Signature = signature
            };
        }

        private static string ParseSignature(NameValueCollection decodedUriParams) =>
            decodedUriParams["sig"] ?? throw new FormatException("Cannot parse signature");

        private static DateTimeOffset ParseExpiryDate(NameValueCollection decodedExistingAbsoluteUriQuery) =>
            DateTimeOffset.TryParse(decodedExistingAbsoluteUriQuery["se"], out var expiryDate)
                ? expiryDate
                : throw new FormatException("Cannot parse expiry date");

        private static PrivateBlobPermission ParseBlobPermission(NameValueCollection query) =>
            query["sp"] switch
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
}
