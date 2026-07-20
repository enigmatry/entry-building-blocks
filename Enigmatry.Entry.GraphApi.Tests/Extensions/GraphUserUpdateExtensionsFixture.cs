using Enigmatry.Entry.GraphApi.Extensions;
using Enigmatry.Entry.GraphApi.Models;
using Microsoft.Graph.Models;
using Shouldly;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests.Extensions;

[Category("unit")]
public class GraphUserUpdateExtensionsFixture
{
    private FakeGraphClient _graph = null!;

    [TearDown]
    public void TearDown() => _graph?.Dispose();

    [Test]
    public async Task UpdateUserPatchesOnlyProvidedDetails()
    {
        _graph = new FakeGraphClientBuilder().WithUser(GraphUser.Some).Build();
        var userDetails = new UserDetails("42", PasswordProfile.Some) { DisplayName = "New Name" };

        var user = await _graph.Client.UpdateUser(userDetails);

        user.ShouldNotBeNull();
        await Verify(_graph.SingleRequestSnapshot());
    }

    [Test]
    public async Task UpdateSignInEmailPatchesMatchingIdentity()
    {
        _graph = new FakeGraphClientBuilder().WithUser(GraphUser.Some).Build();
        var user = new GraphUser { Id = "42", Identities = [ObjectIdentity.SomeEmail] };

        var updated = await _graph.Client.UpdateUserSignInEmailAddress(user, "john@doe.com", "new@doe.com");

        updated.ShouldNotBeNull();
        await Verify(_graph.SingleRequestSnapshot());
    }

    [Test]
    public async Task UpdateSignInEmailWithNoMatchDoesNotPatch()
    {
        _graph = new FakeGraphClientBuilder().Build();
        var user = new GraphUser { Id = "42", Identities = [ObjectIdentity.SomeFederated] };

        var updated = await _graph.Client.UpdateUserSignInEmailAddress(user, "john@doe.com", "new@doe.com");

        updated.ShouldBe(user);
        _graph.Requests.ShouldBeEmpty();
    }
}
