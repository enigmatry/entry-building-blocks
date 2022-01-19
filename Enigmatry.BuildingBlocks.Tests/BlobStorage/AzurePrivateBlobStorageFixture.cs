using Azure.Storage.Blobs;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Web;
using Enigmatry.BuildingBlocks.BlobStorage;
using Enigmatry.BuildingBlocks.BlobStorage.Azure;

namespace Enigmatry.BuildingBlocks.Tests.BlobStorage
{
    [Category("unit")]
    public class AzurePrivateBlobStorageFixture
    {
#pragma warning disable CS8618
        private AzurePrivateBlobStorage _blobStorage;
#pragma warning restore CS8618
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
                       "?sv=2020-08-04&spr=https&se=2022-01-11T16%3A00%3A39Z&sr=b&sp=r" +
                       "&sig=6vhsxACAj6lDMcBAspgKhb9uMDGsb6Rrh3Oj0V%2FLOn0%3D";
            _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeTrue();
        }

        [Test]
        public void VerifySharedResourcePathReturnsFalseWhenPathContainsWrongPermission()
        {
            var path = "https://testaccount.blob.core.windows.net:443" +
                       "/testContainer/testResource.pdf" +
                       "?sv=2020-08-04&spr=https&se=2022-01-11T16%3A00%3A39Z&sr=b&sp=w" +
                       "&sig=6vhsxACAj6lDMcBAspgKhb9uMDGsb6Rrh3Oj0V%2FLOn0%3D";
            _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeFalse();
        }

        [Test]
        public void VerifySharedResourcePathReturnsFalseWhenSignatureIsCorrupted()
        {
            var path = "https://testaccount.blob.core.windows.net:443" +
                       "/testContainer/testResource.pdf" +
                       "?sv=2020-08-04&spr=https&se=2022-01-11T16%3A00%3A39Z&sr=b&sp=r" +
                       "&sig=6vhsxACAj6ldMCBAsPgKhb9uMDGsb6Rrh3Oj0V%2FLOn0%3D";
            _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeFalse();
        }

        [Test]
        public void VerifySharedResourcePathReturnsFalseWhenBlobNameIsDifferent()
        {
            var path = "https://testaccount.blob.core.windows.net:443" +
                       "/testContainer/testResourcee.pdf" +
                       "?sv=2020-08-04&spr=https&se=2022-01-11T16%3A00%3A39Z&sr=b&sp=r" +
                       "&sig=6vhsxACAj6lDMcBAspgKhb9uMDGsb6Rrh3Oj0V%2FLOn0%3D";
            _blobStorage.VerifySharedResourcePath(new Uri(path)).Should().BeFalse();
        }
    }
}

