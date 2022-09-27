using System;
using Azure.Storage.Sas;

namespace Enigmatry.Entry.BlobStorage.Azure
{
    public static class AzureBlobStoragePrivateBlobPermissionExtensions
    {
        public static BlobSasPermissions ToBlobSasPermissions(this PrivateBlobPermission permission) =>
            permission switch
            {
                PrivateBlobPermission.Read => BlobSasPermissions.Read,
                PrivateBlobPermission.Write => BlobSasPermissions.Write,
                PrivateBlobPermission.Delete => BlobSasPermissions.Delete,
                _ => throw new ArgumentOutOfRangeException(nameof(permission), permission, "Invalid type.")
            };
    }
}
