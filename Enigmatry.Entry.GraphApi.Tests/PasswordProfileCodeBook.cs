using Microsoft.Graph.Models;

namespace Enigmatry.Entry.GraphApi.Tests;

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
