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
        var path = "https://testaccount.blob.core.windows.net:443" +
                   "/testContainer/testResource.pdf" +
                   "?sv=2023-11-03&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=r" +
                   "&sig=Z%2FqCBdA073EKBj3PdsX4MWkNchH8Mdot1nf2R2mpVDM%3D";

        _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeTrue();
    }

    [Test]
    public void VerifySharedResourcePathReturnsFalseWhenPathContainsWrongPermission()
    {
        var path = "https://testaccount.blob.core.windows.net:443" +
                   "/testContainer/testResource.pdf" +
                   "?sv=2023-11-03&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=w" +
                   "&sig=Z%2FqCBdA073EKBj3PdsX4MWkNchH8Mdot1nf2R2mpVDM%3D";
        _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeFalse();
    }

    [Test]
    public void VerifySharedResourcePathReturnsFalseWhenSignatureIsCorrupted()
    {
        var path = "https://testaccount.blob.core.windows.net:443" +
                   "/testContainer/testResource.pdf" +
                   "?sv=2023-11-03&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=r" +
                   "&sig=z%2FqCBdA073EKBj3PdsX4MWkNchH8Mdot1nf2R2mpVDM%3D";

        _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeFalse();
    }

    [Test]
    public void VerifySharedResourcePathReturnsFalseWhenBlobNameIsDifferent()
    {
        var path = "https://testaccount.blob.core.windows.net:443" +
                   "/testContainer/testResourcee.pdf" +
                   "?sv=2023-11-03&spr=https&se=2022-08-10T12%3A26%3A47Z&sr=b&sp=r" +
                   "&sig=Z%2FqCBdA073EKBj3PdsX4MWkNchH8Mdot1nf2R2mpVDM%3D";
        _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeFalse();
    }
}
