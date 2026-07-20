using Enigmatry.Entry.GraphApi.Extensions;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using NUnit.Framework;
using Shouldly;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests;

[Category("unit")]
public class GraphClientSearchExtensionsFixture
{
    private static readonly string[] DefaultSelectFields =
        ["id", "displayName", "givenName", "surname", "identities", "accountEnabled", "userPrincipalName", "mail"];

    private FakeGraphClient _graph = null!;

    [SetUp]
    public void SetUp() => _graph = new FakeGraphClient();

    [Test]
    public async Task GetUserById_RequestsUserWithDefaultFieldsCamelCased()
    {
        _graph.UserResponse = new GraphUser { Id = "42" };

        var user = await _graph.Client.GetUserById("42");

        user.ShouldNotBeNull().Id.ShouldBe("42");
        var request = _graph.SingleRequest;
        request.HttpMethod.ShouldBe(Method.GET);
        request.URI.GetLeftPart(UriPartial.Path).ShouldBe("https://graph.microsoft.com/v1.0/users/42");
        request.QueryParameters["%24select"].ShouldBe(DefaultSelectFields);
    }

    [Test]
    public async Task GetUserById_WithCustomSelectExpression_CamelCasesTheSelectedProperties()
    {
        await _graph.Client.GetUserById("42", user => [nameof(user.Id), nameof(user.OtherMails)]);

        string[] expected = ["id", "otherMails"];
        _graph.SingleRequest.QueryParameters["%24select"].ShouldBe(expected);
    }

    [Test]
    public async Task GetUserByIssuerAssignedId_FiltersOnIdentitiesAndReturnsTheSingleMatch()
    {
        var expected = new GraphUser { Id = "42" };
        _graph.UsersResponse = new UserCollectionResponse { Value = [expected] };

        var user = await _graph.Client.GetUserByIssuerAssignedId("john@doe.com", "contoso.onmicrosoft.com");

        user.ShouldBe(expected);
        var request = _graph.SingleRequest;
        request.URI.GetLeftPart(UriPartial.Path).ShouldBe("https://graph.microsoft.com/v1.0/users");
        request.QueryParameters["%24filter"].ShouldBe(
            "identities/any(c:c/issuerAssignedId eq 'john@doe.com' and c/issuer eq 'contoso.onmicrosoft.com')");
        request.QueryParameters["%24select"].ShouldBe(DefaultSelectFields);
    }

    [Test]
    public async Task GetUserByIssuerAssignedId_WhenNoUserMatches_ReturnsNull()
    {
        _graph.UsersResponse = new UserCollectionResponse { Value = [] };

        var user = await _graph.Client.GetUserByIssuerAssignedId("john@doe.com", "contoso.onmicrosoft.com");

        user.ShouldBeNull();
    }

    [Test]
    public async Task GetUsers_RequestsGivenPageSize()
    {
        _graph.UsersResponse = new UserCollectionResponse { Value = [new GraphUser { Id = "42" }] };

        var users = await _graph.Client.GetUsers(pageSize: 5);

        users.Count.ShouldBe(1);
        var request = _graph.SingleRequest;
        request.QueryParameters["%24top"].ShouldBe(5);
        request.QueryParameters["%24select"].ShouldBe(DefaultSelectFields);
    }

    [Test]
    public async Task GetUsers_WhenResponseIsNull_ReturnsEmptyList()
    {
        var users = await _graph.Client.GetUsers();

        users.ShouldBeEmpty();
    }

    [Test]
    public async Task SearchUsers_FiltersOnNameAndEmailProperties()
    {
        await _graph.Client.SearchUsers("Jo");

        _graph.SingleRequest.QueryParameters["%24filter"].ShouldBe(
            "startswith(displayName, 'Jo') or startswith(givenName, 'Jo') or startswith(surname, 'Jo') or startswith(mail, 'Jo') or startswith(userPrincipalName, 'Jo')");
    }

    [Test]
    public async Task SearchUsers_EscapesSingleQuotesInTheQuery()
    {
        await _graph.Client.SearchUsers("O'Brien");

        _graph.SingleRequest.QueryParameters["%24filter"].ShouldBe(
            "startswith(displayName, 'O''Brien') or startswith(givenName, 'O''Brien') or startswith(surname, 'O''Brien') or startswith(mail, 'O''Brien') or startswith(userPrincipalName, 'O''Brien')");
    }

    [Test]
    public async Task GetUserByIssuerAssignedId_EscapesSingleQuotesInTheArguments()
    {
        _graph.UsersResponse = new UserCollectionResponse { Value = [] };

        await _graph.Client.GetUserByIssuerAssignedId("o'brien@doe.com", "contoso.onmicrosoft.com");

        _graph.SingleRequest.QueryParameters["%24filter"].ShouldBe(
            "identities/any(c:c/issuerAssignedId eq 'o''brien@doe.com' and c/issuer eq 'contoso.onmicrosoft.com')");
    }
}
