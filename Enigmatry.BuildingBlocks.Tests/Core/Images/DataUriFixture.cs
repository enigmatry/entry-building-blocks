using Enigmatry.BuildingBlocks.Core.Images;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace Enigmatry.BuildingBlocks.Tests.Core.Images
{
    public class DataUriFixture
    {
        [Test]
        public void DataUriArrayCreationNullGuard() =>
            ShouldThrow<ArgumentNullException>(() => DataUri.CreateFrom(null!, "some type"));

        [Test]
        public void DataUriArrayCreationEmptyGuard() =>
            ShouldThrow<ArgumentOutOfRangeException>(() => DataUri.CreateFrom(Array.Empty<byte>(), "some type"));

        [Test]
        public void DataUriArrayCreationTypeGuard() =>
            ShouldThrow<ArgumentException>(() => DataUri.CreateFrom(new byte[] { 234, 123 }, string.Empty));

        [Test]
        public void DataUriStringCreationNullGuard() =>
            ShouldThrow<ArgumentNullException>(() => DataUri.CreateFrom(null!));

        [Test]
        public void DataUriArrayCreationSuccess()
        {
            var uri = DataUri.CreateFrom(Source.Valid.Bytes, "image/png");

            Source.Valid.PngUri.Should().BeEquivalentTo(uri.ToString());
        }

        [Test]
        [TestCase("Wa9pDZ9A4U2tZbUG")]
        [TestCase("image/png,Wa9pDZ9A4U2tZbUG")]
        [TestCase("data:application/json,Wa9pDZ9A4U2tZbUG")]
        [TestCase("data:image/png,")]
        [TestCase("data:image/png-Wa9pDZ9A4U2tZbUG")]
        public void DataUriGuard(string input) => ShouldThrow<ArgumentException>(() => DataUri.CreateFrom(input));

        [Test]
        public void ToByteArrayConversion()
        {
            var dataUri = DataUri.CreateFrom(Source.Valid.PngUri);

            var bytes = dataUri.ToByteArray();

            Source.Valid.Bytes.Should().BeEquivalentTo(bytes);
        }

        private static void ShouldThrow<TException>(Action act) where TException : Exception =>
            act.Should().Throw<TException>();
    }
}
