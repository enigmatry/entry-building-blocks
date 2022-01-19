using System;

namespace Enigmatry.BuildingBlocks.BlobStorage
{
    public interface IPrivateBlobStorage : IBlobStorage
    {
        string BuildSharedResourcePath(string relativePath, PrivateBlobPermission permission = PrivateBlobPermission.Read);
        bool VerifySharedResourcePath(Uri uri);
    }
}
