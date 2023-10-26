using Enigmatry.Entry.Core.Entities;
using Enigmatry.Entry.EntityFramework.Security;
using FakeItEasy;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Enigmatry.Entry.EntityFramework.Tests;

public class PublishDomainEventsInterceptorTests
{
    private IServiceScope _testScope = null!;
    private TestDbContext _testDbContext = null!;
    private IMediator _testMediator = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        RegisterDbContext(services);
        services.AddLogging(l => l.AddProvider(NullLoggerProvider.Instance));
        services.AddScoped(_ => A.Fake<IMediator>());

        var provider = services.BuildServiceProvider();

        _testScope = provider.CreateScope();
        _testDbContext = _testScope.ServiceProvider.GetRequiredService<TestDbContext>();
        _testMediator = _testScope.ServiceProvider.GetRequiredService<IMediator>();
    }

    [Test]
    public async Task TestDomainEventsArePublished()
    {
        var testEntity1 = new TestEntity();
        var testEntity1CreatedEvent = testEntity1.GetDomainEvents().First();

        var testEntity2 = new TestEntity();
        var testEntity2CreatedEvent = testEntity2.GetDomainEvents().First();

        await _testDbContext.Set<TestEntity>().AddRangeAsync(testEntity1, testEntity2);
        await _testDbContext.SaveChangesAsync();

        A.CallTo(() =>
                _testMediator.Publish(testEntity1CreatedEvent, A.Dummy<CancellationToken>()))
            .MustHaveHappened(1, Times.Exactly);
        A.CallTo(() =>
                _testMediator.Publish(testEntity2CreatedEvent, A.Dummy<CancellationToken>()))
            .MustHaveHappened(1, Times.Exactly);
    }

    [TearDown]
    public void Teardown() => _testScope.Dispose();

    private static void RegisterDbContext(ServiceCollection collection)
    {
        collection.AddSingleton(_ => new EntitiesDbContextOptions
        {
            ConfigurationAssembly = typeof(PublishDomainEventsInterceptorTests).Assembly,
            EntitiesAssembly = typeof(PublishDomainEventsInterceptorTests).Assembly
        });
        collection.AddSingleton<IDbContextAccessTokenProvider, NullDbContextAccessTokenProvider>();
        collection.AddScoped<ISaveChangesInterceptor, PublishDomainEventsInterceptor>();

        collection.AddDbContext<TestDbContext>((sp, optionsBuilder) =>
        {
            optionsBuilder.UseInMemoryDatabase(nameof(PublishDomainEventsInterceptorTests));
            optionsBuilder.AddInterceptors(sp.GetRequiredService<IEnumerable<ISaveChangesInterceptor>>());
        });
    }
}

public class TestDbContext : BaseDbContext
{
    public TestDbContext(EntitiesDbContextOptions entitiesDbContextOptions, DbContextOptions options,
        IDbContextAccessTokenProvider dbContextAccessTokenProvider) : base(entitiesDbContextOptions, options,
        dbContextAccessTokenProvider)
    {
    }
}

public class TestEntity : EntityWithGuidId
{
    public TestEntity()
    {
        AddDomainEvent(new TestEntityCreated(Id));
    }
}

public record TestEntityCreated(Guid Id) : DomainEvent;
