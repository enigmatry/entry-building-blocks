using Microsoft.Graph.Models;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests;

internal static class GraphUserCodeBook
{
    extension(GraphUser)
    {
        public static GraphUser Some => new() { Id = "42" };

        public static GraphUser SomeWithEmailIdentity => new() { Id = "42", Identities = [ObjectIdentity.SomeEmail] };

        public static GraphUser SomeWithFederatedIdentity => new() { Id = "42", Identities = [ObjectIdentity.SomeFederated] };
    }
}
