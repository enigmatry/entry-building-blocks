using Enigmatry.Entry.GraphApi.Extensions;
using Shouldly;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests.Extensions;

[Category("unit")]
public class GraphClientSearchExtensionsFixture
{
    private FakeGraphClient _graph = null!;

    [TearDown]
    public void TearDown() => _graph?.Dispose();

    [Test]
    public async Task GetUserByIdRequestsDefaultFields()
    {
        var some = GraphUser.Some;
        _graph = new FakeGraphClientBuilder().WithUser(some).Build();

        var user = await _graph.Client.GetUserById(some.Id!);

        user.ShouldBe(some);
        await Verify(_graph.SingleRequestSnapshot());
    }

    [Test]
    public async Task GetUserByIdCamelCasesCustomSelect()
    {
        _graph = new FakeGraphClientBuilder().Build();

        await _graph.Client.GetUserById("42", user => [nameof(user.Id), nameof(user.OtherMails)]);

        await Verify(_graph.SingleRequestSnapshot());
    }

    [TestCase("john@doe.com")]
    [TestCase("o'brien@doe.com")]
    public async Task GetUserByIssuerAssignedIdFiltersOnIdentities(string issuerAssignedId)
    {
        var some = GraphUser.Some;
        _graph = new FakeGraphClientBuilder().WithUsers(some).Build();

        var user = await _graph.Client.GetUserByIssuerAssignedId(issuerAssignedId, "contoso.onmicrosoft.com");

        user.ShouldBe(some);
        await Verify(_graph.SingleRequestSnapshot());
    }

    [Test]
    public async Task GetUserByIssuerAssignedIdWithNoMatchReturnsNull()
    {
        _graph = new FakeGraphClientBuilder().WithUsers().Build();

        var user = await _graph.Client.GetUserByIssuerAssignedId("john@doe.com", "contoso.onmicrosoft.com");

        user.ShouldBeNull();
    }

    [Test]
    public async Task GetUsersRequestsPageSize()
    {
        _graph = new FakeGraphClientBuilder().WithUsers(GraphUser.Some).Build();

        var users = await _graph.Client.GetUsers(pageSize: 5);

        users.Count.ShouldBe(1);
        await Verify(_graph.SingleRequestSnapshot());
    }

    [Test]
    public async Task GetUsersWithNullResponseReturnsEmptyList()
    {
        _graph = new FakeGraphClientBuilder().Build();

        var users = await _graph.Client.GetUsers();

        users.ShouldBeEmpty();
    }

    [TestCase("Jo")]
    [TestCase("O'Brien")]
    public async Task SearchUsersFiltersOnNameAndEmail(string query)
    {
        _graph = new FakeGraphClientBuilder().Build();

        await _graph.Client.SearchUsers(query);

        await Verify(_graph.SingleRequestSnapshot());
    }
}
