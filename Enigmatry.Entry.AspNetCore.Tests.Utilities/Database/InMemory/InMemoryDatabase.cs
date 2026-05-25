using Microsoft.EntityFrameworkCore.Storage;
using TimeProvider = Enigmatry.Entry.Infrastructure.TimeProvider;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database.InMemory;

internal static class InMemoryDatabase
{
    public static string Name => "EnigmatryEntryInMemory" + new TimeProvider().UtcNow.Ticks + Guid.NewGuid();
    public static readonly InMemoryDatabaseRoot Root = new();
}
