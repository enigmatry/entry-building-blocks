using Enigmatry.Entry.GraphApi.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Enigmatry.Entry.GraphApi.Tests;

[Category("unit")]
public class GraphClientPhotoExtensionsFixture
{
    private FakeGraphClient _graph = null!;

    [SetUp]
    public void SetUp() => _graph = new FakeGraphClient();

    [Test]
    public async Task GetUserPhoto_RequestsThePhotoContentOfTheGivenUser()
    {
        using var photo = new MemoryStream([1, 2, 3]);
        _graph.StreamResponse = photo;

        var result = await _graph.Client.GetUserPhoto("42");

        result.ShouldBe(photo);
        _graph.SingleRequest.URI.GetLeftPart(UriPartial.Path)
            .ShouldBe("https://graph.microsoft.com/v1.0/users/42/photo/$value");
    }

    [Test]
    public async Task GetCurrentUserPhoto_RequestsThePhotoContentOfTheCurrentUser()
    {
        using var photo = new MemoryStream([1, 2, 3]);
        _graph.StreamResponse = photo;

        var result = await _graph.Client.GetCurrentUserPhoto();

        result.ShouldBe(photo);
        _graph.SingleRequest.URI.GetLeftPart(UriPartial.Path)
            .ShouldBe("https://graph.microsoft.com/v1.0/me/photo/$value");
    }
}
