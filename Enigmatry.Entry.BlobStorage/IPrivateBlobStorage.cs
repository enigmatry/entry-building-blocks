using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage;

[PublicAPI]
public interface IPrivateBlobStorage : IBlobStorage
{
    public string BuildSharedResourcePath(string relativePath, PrivateBlobPermission permission = PrivateBlobPermission.Read);
    public bool VerifySharedResourcePath(Uri uri);
}
