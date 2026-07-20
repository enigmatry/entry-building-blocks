using Enigmatry.Entry.GraphApi.Extensions;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using NUnit.Framework;
using Shouldly;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests.Extensions;

[Category("unit")]
public class GraphUserDeleteExtensionsFixture
{
    private FakeGraphClient _graph = null!;

    [SetUp]
    public void SetUp() => _graph = new FakeGraphClient();

    [Test]
    public async Task RemoveUserByIdSendsDelete()
    {
        await _graph.Client.RemoveUser("42");

        var request = _graph.SingleRequest;
        request.HttpMethod.ShouldBe(Method.DELETE);
        request.URI.GetLeftPart(UriPartial.Path).ShouldBe("https://graph.microsoft.com/v1.0/users/42");
    }

    [Test]
    public async Task RemoveUserByIssuerAssignedIdDeletesTheUserOnce()
    {
        var user = new GraphUser { Id = "42" };
        _graph.UsersResponse = new UserCollectionResponse { Value = [user] };

        var removed = await _graph.Client.RemoveUser("john@doe.com", "contoso.onmicrosoft.com");

        removed.ShouldBe(user);
        _graph.Requests.Count.ShouldBe(2);
        _graph.Requests[0].HttpMethod.ShouldBe(Method.GET);
        _graph.Requests[1].HttpMethod.ShouldBe(Method.DELETE);
        _graph.Requests[1].URI.GetLeftPart(UriPartial.Path).ShouldBe("https://graph.microsoft.com/v1.0/users/42");
    }

    [Test]
    public async Task RemoveUserByIssuerAssignedIdWithUnknownUserReturnsNull()
    {
        _graph.UsersResponse = new UserCollectionResponse { Value = [] };

        var user = await _graph.Client.RemoveUser("john@doe.com", "contoso.onmicrosoft.com");

        user.ShouldBeNull();
        _graph.SingleRequest.HttpMethod.ShouldBe(Method.GET);
    }
}
