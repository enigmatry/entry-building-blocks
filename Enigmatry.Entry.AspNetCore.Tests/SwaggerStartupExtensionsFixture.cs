using System.Reflection;
using Enigmatry.Entry.Swagger;
using Shouldly;

namespace Enigmatry.Entry.AspNetCore.Tests;

[Category("unit")]
public class SwaggerStartupExtensionsFixture
{
    private static readonly MethodInfo[] PublicStaticMethods =
        typeof(SwaggerStartupExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static);

    [Test]
    public void ImplicitGrantFlow_IsNotExposed()
    {
        // OAuth2 Implicit Grant is removed in OAuth 2.1 (BP-1611). No public member may offer it.
        var implicitGrantMethods = PublicStaticMethods
            .Where(m => m.Name.Contains("ImplicitGrant", StringComparison.Ordinal))
            .Select(m => m.Name)
            .ToArray();

        implicitGrantMethods.ShouldBeEmpty();
    }

    [Test]
    public void AddEntrySwaggerWithAuthorizationCode_IsAvailableAndNotObsolete()
    {
        var method = PublicStaticMethods
            .SingleOrDefault(m => m.Name == nameof(SwaggerStartupExtensions.AddEntrySwaggerWithAuthorizationCode));

        method.ShouldNotBeNull();
        method.GetCustomAttribute<ObsoleteAttribute>().ShouldBeNull();
    }
}
