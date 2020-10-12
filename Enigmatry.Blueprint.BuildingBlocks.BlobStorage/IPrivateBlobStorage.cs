namespace Enigmatry.Blueprint.BuildingBlocks.BlobStorage
{
    public interface IPrivateBlobStorage : IBlobStorage
    {
        string BuildSharedResourcePath(string path);
    }
}
