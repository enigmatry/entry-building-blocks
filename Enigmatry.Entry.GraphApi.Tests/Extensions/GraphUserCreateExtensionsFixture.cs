using Enigmatry.Entry.GraphApi.Extensions;
using Microsoft.Graph.Models;
using Shouldly;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests.Extensions;

[Category("unit")]
public class GraphUserCreateExtensionsFixture
{
    private FakeGraphClient _graph = null!;

    [TearDown]
    public void TearDown() => _graph?.Dispose();

    [Test]
    public async Task AddUserPostsToUsersEndpoint()
    {
        var created = GraphUser.Some;
        _graph = new FakeGraphClientBuilder().WithUser(created).Build();

        var user = await _graph.Client.AddUser(
            "John Doe", ObjectIdentity.SomeEmail, PasswordProfile.Some, "DisablePasswordExpiration");

        user.ShouldBe(created);
        await Verify(_graph.SingleRequestSnapshot());
    }
}
