using Azure.Storage.Blobs;
using Enigmatry.BuildingBlocks.Core.Settings;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using Enigmatry.BuildingBlocks.BlobStorage;
using Enigmatry.BuildingBlocks.BlobStorage.Azure;

namespace Enigmatry.BuildingBlocks.Tests.BlobStorage
{
    [Category("unit")]
    public class AzureBlobStorageFixture
    {
#pragma warning disable CS8618
        private AzureBlobStorage _blobStorage;
#pragma warning restore CS8618
        private const string AccountName = "testaccount";
        private const string ContainerName = "testContainer";
        private const string ResourceName = "testResource.pdf";


        [SetUp]
        public void Setup()
        {
            var settings = Options.Create(new AzureBlobStorageSettings()
            {
                AccountName = AccountName,
                AccountKey = "8ab4k8YGxQMhcAhN8S3M9R2COIVbSD3yC7HIuh5GKw1+CPamPjPQskRU97uZfKvK7C/XrZyCeDP2XUIedwRYYw==",
                CacheTimeout = 600
            });

            var container = new BlobContainerClient(settings.Value.ConnectionString, ContainerName);
            _blobStorage = new AzureBlobStorage(container, settings.Value);
        }

        [Test]
        public void TestResourcePath()
        {
            var path = _blobStorage.BuildResourcePath(ResourceName);
            path.Should().Be($"https://{AccountName}.blob.core.windows.net/{ContainerName}/{ResourceName}");
        }

        [Test]
        public void TestBlobHttpHeaders()
        {
            var blob = new BlobClient(new Uri($"https://{AccountName}.blob.core.windows.net/{ContainerName}/{ResourceName}"));
            var headers = _blobStorage.ConfigureBlobHttpHeadersAsync(blob);
            headers.CacheControl.Should().Be("public, max-age=600");
            headers.ContentType.Should().Be("application/pdf");
        }
    }
}

