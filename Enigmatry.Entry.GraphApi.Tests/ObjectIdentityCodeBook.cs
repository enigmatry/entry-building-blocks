using Microsoft.Graph.Models;

namespace Enigmatry.Entry.GraphApi.Tests;

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
