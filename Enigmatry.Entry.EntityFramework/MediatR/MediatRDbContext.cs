using Enigmatry.Entry.Core.Entities;
using Enigmatry.Entry.EntityFramework.Security;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enigmatry.Entry.EntityFramework.MediatR
{
    [UsedImplicitly]
    public abstract class MediatRDbContext : BaseDbContext
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MediatRDbContext> _logger;

        protected MediatRDbContext(EntitiesDbContextOptions entitiesDbContextOptions, DbContextOptions options, IMediator mediator,
            ILogger<MediatRDbContext> logger, IDbContextAccessTokenProvider dbContextAccessTokenProvider) :
            base(entitiesDbContextOptions, options, dbContextAccessTokenProvider)
        {
            _mediator = mediator;
            _logger = logger;
        }

        protected override Task Dispatch(IEnumerable<DomainEvent> domainEvents) =>
            _mediator.DispatchDomainEventsAsync(domainEvents, _logger);
    }
}
