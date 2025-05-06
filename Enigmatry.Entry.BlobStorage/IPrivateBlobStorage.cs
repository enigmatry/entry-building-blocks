using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage;

[PublicAPI]
public interface IPrivateBlobStorage : IBlobStorage
{
    string BuildSharedResourcePath(string relativePath, PrivateBlobPermission permission = PrivateBlobPermission.Read);
    bool VerifySharedResourcePath(Uri uri);
}
