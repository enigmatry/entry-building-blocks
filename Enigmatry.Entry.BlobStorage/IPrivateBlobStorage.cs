using Enigmatry.Entry.BlobStorage.Models;
using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage;

[PublicAPI]
public interface IPrivateBlobStorage : IBlobStorage
{
    [Obsolete("Use BuildSharedResourcePath with ContentDispositionSettings instead.")]
    string BuildSharedResourcePath(string relativePath, string fileName, PrivateBlobPermission permission = PrivateBlobPermission.Read);
    string BuildSharedResourcePath(string relativePath, ContentDisposition? settings = null, PrivateBlobPermission permission = PrivateBlobPermission.Read);
    bool VerifySharedResourcePath(Uri uri);
}
