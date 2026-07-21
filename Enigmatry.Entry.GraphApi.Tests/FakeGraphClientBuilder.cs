using Microsoft.Graph.Models;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests;

internal sealed class FakeGraphClientBuilder
{
    private GraphUser? _user;
    private UserCollectionResponse? _users;
    private Stream? _photo;

    public FakeGraphClientBuilder WithUser(GraphUser user)
    {
        _user = user;
        return this;
    }

    public FakeGraphClientBuilder WithUsers(params GraphUser[] users)
    {
        _users = new UserCollectionResponse { Value = [.. users] };
        return this;
    }

    public FakeGraphClientBuilder WithPhoto(Stream photo)
    {
        _photo = photo;
        return this;
    }

    public FakeGraphClient Build() => new(_user, _users, _photo);
}
