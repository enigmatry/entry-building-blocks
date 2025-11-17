using Enigmatry.Entry.BlobStorage.Models;
using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage;

[PublicAPI]
public interface IPrivateBlobStorage : IBlobStorage
{
    string BuildSharedResourcePath(string relativePath, PrivateBlobPermission permission = PrivateBlobPermission.Read, BlobResponseHeadersOverride? responseHeaders = null);
    bool VerifySharedResourcePath(Uri uri);
}
