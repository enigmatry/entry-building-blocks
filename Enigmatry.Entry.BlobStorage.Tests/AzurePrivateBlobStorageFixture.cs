using System.Web;
using Azure.Storage.Blobs;
using Enigmatry.Entry.BlobStorage.Azure;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

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

    [Test]
    [TestCase(PrivateBlobPermission.Read)]
    [TestCase(PrivateBlobPermission.Write)]
    [TestCase(PrivateBlobPermission.Delete)]
    public void TestSharedResourcePath(PrivateBlobPermission permission)
    {
        var path = _blobStorage.BuildSharedResourcePath(ResourceName, permission);
        path.Should().Contain($"https://{AccountName}.blob.core.windows.net:443/{ContainerName}/{ResourceName}");

        var queryParams = HttpUtility.ParseQueryString(path);
        var expiresOn = DateTime.Parse(queryParams["se"]!).ToUniversalTime();

        expiresOn.Should().BeAfter(DateTime.UtcNow);
        expiresOn.Should().BeLessThan(_sasDuration).After(DateTime.UtcNow);
    }

    [Test]
    public void VerifySharedResourcePathReturnsTrueWhenPathSignatureIsValid()
    {
        // if this test starts to fail with the upgrade of Azure.Storage.Blob nuget
        // it might be caused by the change in the algorithm of the signature
        // the fix is to grab the new signature in the debugger and update the test 
        var path = "https://testaccount.blob.core.windows.net:443" +
                   "/testContainer/testResource.pdf" +
                   "?sv=2025-01-05&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=r" +
                   "&sig=5gu1L%2BkjfDonXKnS8lJ8cJsOK4AU8CVacnxSyou2L0w%3D";

        _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeTrue();
    }

    [Test]
    public void VerifySharedResourcePathReturnsFalseWhenPathContainsWrongPermission()
    {
        var path = "https://testaccount.blob.core.windows.net:443" +
                   "/testContainer/testResource.pdf" +
                   "?sv=2023-11-03&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=w" +
                   "&sig=2trbBGJP8FKWPmOwgxlNyGDgCPZhv9XRXpif143gwbc=";
        _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeFalse();
    }

    [Test]
    public void VerifySharedResourcePathReturnsFalseWhenSignatureIsCorrupted()
    {
        var path = "https://testaccount.blob.core.windows.net:443" +
                   "/testContainer/testResource.pdf" +
                   "?sv=2023-11-03&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=r" +
                   "&sig=2TrbBGJP8FKWPmOwgxlNyGDgCPZhv9XRXpif143gwbc=";

        _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeFalse();
    }

    [Test]
    public void VerifySharedResourcePathReturnsFalseWhenBlobNameIsDifferent()
    {
        var path = "https://testaccount.blob.core.windows.net:443" +
                   "/testContainer/testResourcee.pdf" +
                   "?sv=2023-11-03&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=r" +
                   "&sig=2trbBGJP8FKWPmOwgxlNyGDgCPZhv9XRXpif143gwbc=";
        _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeFalse();
    }
}
