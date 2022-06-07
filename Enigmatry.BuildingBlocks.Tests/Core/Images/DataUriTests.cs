using Enigmatry.BuildingBlocks.Core.Images;
using NUnit.Framework;
using System;

namespace Enigmatry.BuildingBlocks.Tests.Core.Images
{
    [TestFixture]
    public class DataUriTests
    {
        [Test]
        public void DataUriArrayCreationNullGuard() =>
            Assert.Throws<ArgumentNullException>(() => DataUri.CreateFrom(null!, "some type"));

        [Test]
        public void DataUriArrayCreationEmptyGuard() =>
            Assert.Throws<ArgumentOutOfRangeException>(() => DataUri.CreateFrom(Array.Empty<byte>(), "some type"));

        [Test]
        public void DataUriArrayCreationTypeGuard() =>
            Assert.Throws<ArgumentException>(() => DataUri.CreateFrom(new byte[] { 234, 123 }, string.Empty));

        [Test]
        public void DataUriStringCreationNullGuard() =>
            Assert.Throws<ArgumentNullException>(() => DataUri.CreateFrom(null!));

        [Test]
        public void DataUriArrayCreationSuccess()
        {
            var uri = DataUri.CreateFrom(Source.Valid.Bytes, "image/png");

            Assert.That(Source.Valid.PngUri, Is.EqualTo(uri.ToString()));
        }

        [Test]
        [TestCase("Wa9pDZ9A4U2tZbUG")]
        [TestCase("image/png,Wa9pDZ9A4U2tZbUG")]
        [TestCase("data:application/json,Wa9pDZ9A4U2tZbUG")]
        [TestCase("data:image/png,")]
        [TestCase("data:image/png-Wa9pDZ9A4U2tZbUG")]
        public void DataUriGuard(string input) => Assert.Throws<ArgumentException>(() => DataUri.CreateFrom(input));

        [Test]
        public void ToByteArrayConversion()
        {
            var dataUri = DataUri.CreateFrom(Source.Valid.PngUri);

            var bytes = dataUri.ToByteArray();

            Assert.That(Source.Valid.Bytes, Is.EqualTo(bytes));
        }
    }
}
