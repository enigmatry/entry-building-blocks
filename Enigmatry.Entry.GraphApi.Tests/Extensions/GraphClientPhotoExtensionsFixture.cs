using Enigmatry.Entry.GraphApi.Extensions;
using Shouldly;

namespace Enigmatry.Entry.GraphApi.Tests.Extensions;

[Category("unit")]
public class GraphClientPhotoExtensionsFixture
{
    private FakeGraphClient _graph = null!;

    [TearDown]
    public void TearDown() => _graph?.Dispose();

    [TestCase("42", "https://graph.microsoft.com/v1.0/users/42/photo/$value")]
    [TestCase(null, "https://graph.microsoft.com/v1.0/me/photo/$value")]
    public async Task GetPhotoRequestsPhotoContent(string? userId, string expectedUrl)
    {
        using var photo = new MemoryStream([1, 2, 3]);
        _graph = new FakeGraphClientBuilder().WithPhoto(photo).Build();

        var result = userId == null
            ? await _graph.Client.GetCurrentUserPhoto()
            : await _graph.Client.GetUserPhoto(userId);

        result.ShouldBe(photo);
        _graph.SingleRequest.URI.GetLeftPart(UriPartial.Path).ShouldBe(expectedUrl);
    }
}
