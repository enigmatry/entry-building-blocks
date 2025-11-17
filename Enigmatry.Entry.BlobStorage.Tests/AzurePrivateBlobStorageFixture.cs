using Azure.Storage.Blobs;
using Enigmatry.Entry.BlobStorage.Azure;
using Enigmatry.Entry.BlobStorage.Models;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shouldly;

namespace Enigmatry.Entry.BlobStorage.Tests;

[Category("unit")]
public class AzurePrivateBlobStorageFixture
{
    private AzurePrivateBlobStorage _blobStorage;
    private const string AccountName = "testaccount";
    private const string ContainerName = "testContainer";
    private const string ResourceName = "testResource.pdf";
    private readonly TimeSpan _sasDuration = TimeSpan.FromSeconds(120);

    [SetUp]
    public void Setup()
    {
        var settings = Options.Create(new AzureBlobStorageSettings()
        {
            AccountName = AccountName,
            // Dummy Account key
            AccountKey = "8ab4k8YGxQMhcAhN8S3M9R2COIVbSD3yC7HIuh5GKw1+CPamPjPQskRU97uZfKvK7C/XrZyCeDP2XUIedwRYYw==",
            CacheTimeout = 600,
            SasDuration = _sasDuration
        });

        var container = new BlobContainerClient(settings.Value.ConnectionString, ContainerName);
        _blobStorage = new AzurePrivateBlobStorage(container, settings.Value);
    }

    [TestCase(PrivateBlobPermission.Read)]
    [TestCase(PrivateBlobPermission.Write)]
    [TestCase(PrivateBlobPermission.Delete)]
    public void BuildSharedResourcePathWithPermission(PrivateBlobPermission permission)
    {
        var path = _blobStorage.BuildSharedResourcePath(ResourceName, permission: permission);
        path.ShouldStartWith($"https://{AccountName}.blob.core.windows.net:443/{ContainerName}/{ResourceName}");

        var isSasUriValid = AzureBlobSharedUri.TryParse(new Uri(path), out var sasUri);
        isSasUriValid.ShouldBeTrue();

        var expiresOn = sasUri.ExpiresOn.ToUniversalTime();
        expiresOn.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
        expiresOn.ShouldBeLessThan(DateTimeOffset.UtcNow.Add(_sasDuration));
    }

    [TestCase("test-file.pdf", "test-file.pdf")]
    [TestCase("//test-file//.pdf", "__test-file__.pdf")]
    [TestCase("test,file.pdf", "test,file.pdf")]
    public void BuildSharedResourcePathWithFileName(string fileName, string expectedFileName)
    {
        var path = _blobStorage.BuildSharedResourcePath(ResourceName, fileName);
        path.ShouldStartWith($"https://{AccountName}.blob.core.windows.net:443/{ContainerName}/{ResourceName}");

        var isSasUriValid = AzureBlobSharedUri.TryParse(new Uri(path), out var sasUri);
        isSasUriValid.ShouldBeTrue();
        sasUri.GetContentDisposition()?.FileName.ShouldBe(expectedFileName);
    }

    [TestCase("test-file.pdf", ContentDispositionType.Attachment, "attachment; filename=\"test-file.pdf\"")]
    [TestCase("test-file.pdf", ContentDispositionType.Inline, "inline; filename=\"test-file.pdf\"")]
    public void BuildSharedResourcePathWithContentDisposition(string fileName, ContentDispositionType type, string expectedContentDisposition)
    {
        var settings = new ContentDisposition(fileName, type);
        var path = _blobStorage.BuildSharedResourcePath(ResourceName, settings);
        path.ShouldStartWith($"https://{AccountName}.blob.core.windows.net:443/{ContainerName}/{ResourceName}");

        var isSasUriValid = AzureBlobSharedUri.TryParse(new Uri(path), out var sasUri);
        isSasUriValid.ShouldBeTrue();

        sasUri.ContentDisposition.ShouldBe(expectedContentDisposition);
        var contentDisposition = sasUri.GetContentDisposition();
        contentDisposition.ShouldNotBeNull();
        contentDisposition.FileName.ShouldBe(fileName);
        contentDisposition.Type.ShouldBe(type);
    }

    [Test]
    public void VerifySharedResourcePathReturnsTrueWhenPathSignatureIsValid()
    {
        // if this test starts to fail with the upgrade of Azure.Storage.Blob nuget
        // it might be caused by the change in the algorithm of the signature
        // the fix is to grab the new signature in the debugger and update the test 
        var path = $"https://{AccountName}.blob.core.windows.net:443" +
                   $"/{ContainerName}/{ResourceName}" +
                   "?sv=2025-11-05&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=r" +
                   "&sig=VlZUrh%2FLFP2UDGPDcq5XcDgnG0uxR3m4kMOyv1GyMT0%3D";

        _blobStorage.VerifySharedResourcePath(new Uri(path)).ShouldBeTrue();
    }

    [Test]
    public void VerifySharedResourcePathReturnsFalseWhenPathContainsWrongPermission()
    {
        var path = $"https://{AccountName}.blob.core.windows.net:443" +
                   $"/{ContainerName}/{ResourceName}" +
                   "?sv=2023-11-03&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=w" +
                   "&sig=2trbBGJP8FKWPmOwgxlNyGDgCPZhv9XRXpif143gwbc=";
        _blobStorage.VerifySharedResourcePath(new Uri(path)).ShouldBeFalse();
    }

    [Test]
    public void VerifySharedResourcePathReturnsFalseWhenSignatureIsCorrupted()
    {
        var path = $"https://{AccountName}.blob.core.windows.net:443" +
                   $"/{ContainerName}/{ResourceName}" +
                   "?sv=2023-11-03&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=r" +
                   "&sig=2TrbBGJP8FKWPmOwgxlNyGDgCPZhv9XRXpif143gwbc=";

        _blobStorage.VerifySharedResourcePath(new Uri(path)).ShouldBeFalse();
    }

    [Test]
    public void VerifySharedResourcePathReturnsFalseWhenBlobNameIsDifferent()
    {
        var path = $"https://{AccountName}.blob.core.windows.net:443" +
                   $"/{ContainerName}/testResourcee.pdf" +
                   "?sv=2023-11-03&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=r" +
                   "&sig=2trbBGJP8FKWPmOwgxlNyGDgCPZhv9XRXpif143gwbc=";
        _blobStorage.VerifySharedResourcePath(new Uri(path)).ShouldBeFalse();
    }
}
