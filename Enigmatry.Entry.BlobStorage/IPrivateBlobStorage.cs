using System;

namespace Enigmatry.Entry.BlobStorage
{
    public interface IPrivateBlobStorage : IBlobStorage
    {
        string BuildSharedResourcePath(string relativePath, PrivateBlobPermission permission = PrivateBlobPermission.Read);
        bool VerifySharedResourcePath(Uri uri);
    }
}
