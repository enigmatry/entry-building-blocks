using Enigmatry.BuildingBlocks.Core.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.EntityFramework
{
    [UsedImplicitly]
    public class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly ILogger<EntityFrameworkUnitOfWork> _logger;
        private bool _cancelSaving;

        public EntityFrameworkUnitOfWork(DbContext context, ILogger<EntityFrameworkUnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_cancelSaving)
            {
                _logger.LogWarning("Not saving database changes since saving was cancelled.");
                return 0;
            }

            var numberOfChanges = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug(
                $"{numberOfChanges} of changed were saved to database {_context.Database.GetDbConnection().Database}");
            return numberOfChanges;
        }

        public void CancelSaving() => _cancelSaving = true;
    }
}
