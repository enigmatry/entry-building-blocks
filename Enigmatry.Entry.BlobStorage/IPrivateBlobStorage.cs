using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage;

[PublicAPI]
public interface IPrivateBlobStorage : IBlobStorage
{
    string BuildSharedResourcePath(string relativePath, string? fileName = null, PrivateBlobPermission permission = PrivateBlobPermission.Read);
    bool VerifySharedResourcePath(Uri uri);
}
