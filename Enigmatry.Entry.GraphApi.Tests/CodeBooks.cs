using Microsoft.Graph.Models;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests;

internal static class GraphUserCodeBook
{
    extension(GraphUser)
    {
        public static GraphUser Some => new() { Id = "42" };
    }
}

internal static class ObjectIdentityCodeBook
{
    extension(ObjectIdentity)
    {
        public static ObjectIdentity SomeEmail => new()
        {
            SignInType = "emailAddress",
            Issuer = "contoso.onmicrosoft.com",
            IssuerAssignedId = "john@doe.com"
        };

        public static ObjectIdentity SomeFederated => new()
        {
            SignInType = "federated",
            Issuer = "contoso.onmicrosoft.com",
            IssuerAssignedId = "john@doe.com"
        };
    }
}

internal static class PasswordProfileCodeBook
{
    extension(PasswordProfile)
    {
        public static PasswordProfile Some => new()
        {
            Password = "some-password",
            ForceChangePasswordNextSignIn = false
        };
    }
}
