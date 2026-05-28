using Autofac;
using Microsoft.EntityFrameworkCore;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities.Database.InMemory;

public class InMemoryDbModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var options = new DbContextOptionsBuilder().UseInMemoryDatabase(InMemoryDatabase.Name, InMemoryDatabase.Root)
            .Options;
        builder.Register(_ => options).AsSelf().InstancePerLifetimeScope();
    }
}
