using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enigmatry.Entry.Core.Entities;
using Enigmatry.Entry.EntityFramework.MediatR;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.EntityFramework;

[UsedImplicitly]
public class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;
    private readonly ILogger<PublishDomainEventsInterceptor> _logger;
    private IEnumerable<DomainEvent> _domainEvents = Enumerable.Empty<DomainEvent>();

    public PublishDomainEventsInterceptor(IMediator mediator, ILogger<PublishDomainEventsInterceptor> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        GatherDomainEventsFromContext(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = new())
    {
        GatherDomainEventsFromContext(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        var task = Task.Run(() => PublishDomainEvents());
        task.GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new())
    {
        await PublishDomainEvents(cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void GatherDomainEventsFromContext(DbContext? dbContext)
    {
        // We need to gather domain events before saving, so that we include events
        // for deleted entities (otherwise they are lost due to deletion of the object from context)
        if (dbContext != null)
        {
            _domainEvents = dbContext.GatherDomainEventsFromContext();
        }
    }

    private async Task PublishDomainEvents(CancellationToken cancellationToken = new()) =>
        // Publish Domain Events collection. 
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
        await _mediator.PublishDomainEventsAsync(_domainEvents, _logger, cancellationToken);
}
