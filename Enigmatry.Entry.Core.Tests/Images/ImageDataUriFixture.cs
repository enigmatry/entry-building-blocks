using Enigmatry.Entry.Core.Images;
using FluentAssertions;
using NUnit.Framework;

namespace Enigmatry.Entry.Core.Tests.Images;

[Category("unit")]
public class ImageDataUriFixture
{
    private static (string Uri, byte[] Bytes) ValidImage => (Uri: "data:image/png,e+o=", Bytes: new byte[] { 123, 234 });

    [Test]
    public void NullGuard()
    {
        Action act = () => _ = ImageDataUri.CreateFrom(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    [TestCase("Wa9pDZ9A4U2tZbUG")]
    [TestCase("image/png,Wa9pDZ9A4U2tZbUG")]
    [TestCase("data:application/json,Wa9pDZ9A4U2tZbUG")]
    [TestCase("data:image/png,")]
    [TestCase("data:image/png-Wa9pDZ9A4U2tZbUG")]
    public void DataGuard(string input)
    {
        Action act = () => _ = ImageDataUri.CreateFrom(input);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ToByteArrayConversion()
    {
        var dataUri = ImageDataUri.CreateFrom(ValidImage.Uri);

        var bytes = dataUri.ToByteArray();

        bytes.Should().BeEquivalentTo(ValidImage.Bytes);
    }
}
