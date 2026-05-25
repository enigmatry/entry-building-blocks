using Enigmatry.Entry.AspNetCore.Tests.Utilities.TestServer;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database.InMemory;

public static class TestRunner
{
    public static async Task Run(IEnvironment environment, Func<IEnvironment, HttpClient, Task> test)
    {
        using var client = await environment.SetupClient();
        await test(environment, client);
    }
}
