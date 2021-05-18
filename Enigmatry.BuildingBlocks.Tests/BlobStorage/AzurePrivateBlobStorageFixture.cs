using Azure.Storage.Blobs;
using Enigmatry.BuildingBlocks.Azure.BlobStorage;
using Enigmatry.BuildingBlocks.Core.Settings;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Web;

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
        private const int SasDuration = 120;

        [SetUp]
        public void Setup()
        {
            var settings = Options.Create(new AzureBlobStorageSettings()
            {
                AccountName = AccountName,
                // Dummy Account key
                AccountKey = "8ab4k8YGxQMhcAhN8S3M9R2COIVbSD3yC7HIuh5GKw1+CPamPjPQskRU97uZfKvK7C/XrZyCeDP2XUIedwRYYw==",
                CacheTimeout = 600,
                SasDuration = SasDuration
            });

            var container = new BlobContainerClient(settings.Value.ConnectionString, ContainerName);
            _blobStorage = new AzurePrivateBlobStorage(container, settings);
        }

        [Test]
        public void TestSharedResourcePath()
        {
            var path = _blobStorage.BuildSharedResourcePath(ResourceName);
            path.Should().Contain($"https://{AccountName}.blob.core.windows.net:443/{ContainerName}/{ResourceName}");

            var parameters = HttpUtility.ParseQueryString(path);
            var startDateTime = DateTime.Parse(parameters["st"]!).ToUniversalTime();
            var endDateTime = DateTime.Parse(parameters["se"]!).ToUniversalTime();

            startDateTime.Should().BeBefore(DateTime.UtcNow);
            endDateTime.Should().BeAfter(DateTime.UtcNow);
            endDateTime.Should().BeLessThan(SasDuration.Seconds()).After(DateTime.UtcNow);
        }
    }
}

