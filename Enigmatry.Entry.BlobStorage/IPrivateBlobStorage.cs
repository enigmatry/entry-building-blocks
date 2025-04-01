namespace Enigmatry.Entry.BlobStorage;

public interface IPrivateBlobStorage : IBlobStorage
{
    public string BuildSharedResourcePath(string relativePath, PrivateBlobPermission permission = PrivateBlobPermission.Read);
    public bool VerifySharedResourcePath(Uri uri);
}
