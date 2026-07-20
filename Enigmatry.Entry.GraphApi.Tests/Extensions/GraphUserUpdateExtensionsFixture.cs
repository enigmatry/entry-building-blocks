using Enigmatry.Entry.GraphApi.Extensions;
using Enigmatry.Entry.GraphApi.Models;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using NUnit.Framework;
using Shouldly;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests.Extensions;

[Category("unit")]
public class GraphUserUpdateExtensionsFixture
{
    private FakeGraphClient _graph = null!;

    [SetUp]
    public void SetUp() => _graph = new FakeGraphClient();

    [Test]
    public async Task UpdateUserPatchesOnlyTheProvidedDetails()
    {
        _graph.UserResponse = new GraphUser { Id = "42" };
        var userDetails = new UserDetails("42", new PasswordProfile { Password = "some-password" })
        {
            DisplayName = "New Name"
        };

        var user = await _graph.Client.UpdateUser(userDetails);

        user.ShouldNotBeNull();
        var request = _graph.SingleRequest;
        request.HttpMethod.ShouldBe(Method.PATCH);
        request.URI.GetLeftPart(UriPartial.Path).ShouldBe("https://graph.microsoft.com/v1.0/users/42");

        var body = _graph.SingleRequestJsonBody();
        body.GetProperty("displayName").GetString().ShouldBe("New Name");
        body.GetProperty("passwordProfile").GetProperty("password").GetString().ShouldBe("some-password");
        body.TryGetProperty("passwordPolicies", out _).ShouldBeFalse();
        body.TryGetProperty("identities", out _).ShouldBeFalse();
    }

    [Test]
    public async Task UpdateUserSignInEmailAddressPatchesTheMatchingIdentity()
    {
        _graph.UserResponse = new GraphUser { Id = "42" };
        var user = new GraphUser
        {
            Id = "42",
            Identities =
            [
                new ObjectIdentity { SignInType = "emailAddress", IssuerAssignedId = "old@doe.com" }
            ]
        };

        var updated = await _graph.Client.UpdateUserSignInEmailAddress(user, "old@doe.com", "new@doe.com");

        updated.ShouldNotBeNull();
        var request = _graph.SingleRequest;
        request.HttpMethod.ShouldBe(Method.PATCH);
        request.URI.GetLeftPart(UriPartial.Path).ShouldBe("https://graph.microsoft.com/v1.0/users/42");

        var identities = _graph.SingleRequestJsonBody().GetProperty("identities").EnumerateArray().ToList();
        identities.Count.ShouldBe(1);
        identities[0].GetProperty("issuerAssignedId").GetString().ShouldBe("new@doe.com");
    }

    [Test]
    public async Task UpdateUserSignInEmailAddressWithNoMatchReturnsTheUserWithoutPatching()
    {
        var user = new GraphUser
        {
            Id = "42",
            Identities = [new ObjectIdentity { SignInType = "federated", IssuerAssignedId = "old@doe.com" }]
        };

        var updated = await _graph.Client.UpdateUserSignInEmailAddress(user, "old@doe.com", "new@doe.com");

        updated.ShouldBe(user);
        _graph.Requests.ShouldBeEmpty();
    }
}
