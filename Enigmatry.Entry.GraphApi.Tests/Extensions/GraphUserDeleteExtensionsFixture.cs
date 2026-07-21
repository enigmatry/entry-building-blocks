using Enigmatry.Entry.GraphApi.Extensions;
using Microsoft.Kiota.Abstractions;
using Shouldly;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests.Extensions;

[Category("unit")]
public class GraphUserDeleteExtensionsFixture
{
    private FakeGraphClient _graph = null!;

    [TearDown]
    public void TearDown() => _graph?.Dispose();

    [Test]
    public async Task RemoveUserByIdSendsDelete()
    {
        _graph = new FakeGraphClientBuilder().Build();

        await _graph.Client.RemoveUser("42");

        await Verify(_graph.SingleRequestSnapshot());
    }

    [Test]
    public async Task RemoveUserByIssuerAssignedIdDeletesUserOnce()
    {
        var some = GraphUser.Some;
        _graph = new FakeGraphClientBuilder().WithUsers(some).Build();

        var removed = await _graph.Client.RemoveUser("john@doe.com", "contoso.onmicrosoft.com");

        removed.ShouldBe(some);
        await Verify(_graph.RequestsSnapshot());
    }

    [Test]
    public async Task RemoveUserByIssuerAssignedIdWithUnknownUserReturnsNull()
    {
        _graph = new FakeGraphClientBuilder().WithUsers().Build();

        var user = await _graph.Client.RemoveUser("john@doe.com", "contoso.onmicrosoft.com");

        user.ShouldBeNull();
        _graph.SingleRequest.HttpMethod.ShouldBe(Method.GET);
    }
}
