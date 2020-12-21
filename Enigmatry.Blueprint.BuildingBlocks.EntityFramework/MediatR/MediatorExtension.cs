using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Enigmatry.Blueprint.BuildingBlocks.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Blueprint.BuildingBlocks.EntityFramework.MediatR
{
    internal static class MediatorExtension
    {
        public static IEnumerable<DomainEvent> GatherDomainEventsFromContext(this DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.GetDomainEvents().Any()).ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.GetDomainEvents())
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            return domainEvents;
        }

        public static async Task DispatchDomainEventsAsync(this IMediator mediator, IEnumerable<DomainEvent> domainEvents, ILogger logger)
        {
            var stopWatch = Stopwatch.StartNew();
            // sequentially publish domain events to avoid problems with same DbContext used by different threads 
            // fixes problem "A second operation started on this context before a previous operation completed"
            // this happens when one event handler is doing DbContext saving while some other one is doing the reading
            foreach (DomainEvent domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                logger.LogDebug("Time to publish domain event - {DomainEvent}: {Time}s", domainEvent.GetType(), ts.TotalSeconds);
                stopWatch.Restart();
            }
        }
    }
}
