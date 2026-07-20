using Enigmatry.Entry.GraphApi.Extensions;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using NUnit.Framework;
using Shouldly;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests.Extensions;

[Category("unit")]
public class GraphUserCreateExtensionsFixture
{
    private FakeGraphClient _graph = null!;

    [SetUp]
    public void SetUp() => _graph = new FakeGraphClient();

    [Test]
    public async Task AddUserPostsTheUserToTheUsersEndpoint()
    {
        var created = new GraphUser { Id = "42" };
        _graph.UserResponse = created;
        var identity = new ObjectIdentity
        {
            SignInType = "emailAddress",
            Issuer = "contoso.onmicrosoft.com",
            IssuerAssignedId = "john@doe.com"
        };
        var passwordProfile = new PasswordProfile { Password = "some-password", ForceChangePasswordNextSignIn = false };

        var user = await _graph.Client.AddUser("John Doe", identity, passwordProfile, "DisablePasswordExpiration");

        user.ShouldBe(created);
        var request = _graph.SingleRequest;
        request.HttpMethod.ShouldBe(Method.POST);
        request.URI.GetLeftPart(UriPartial.Path).ShouldBe("https://graph.microsoft.com/v1.0/users");

        var body = _graph.SingleRequestJsonBody();
        body.GetProperty("displayName").GetString().ShouldBe("John Doe");
        body.GetProperty("passwordPolicies").GetString().ShouldBe("DisablePasswordExpiration");
        body.GetProperty("passwordProfile").GetProperty("password").GetString().ShouldBe("some-password");
        var identities = body.GetProperty("identities").EnumerateArray().ToList();
        identities.Count.ShouldBe(1);
        identities[0].GetProperty("signInType").GetString().ShouldBe("emailAddress");
        identities[0].GetProperty("issuerAssignedId").GetString().ShouldBe("john@doe.com");
    }
}
