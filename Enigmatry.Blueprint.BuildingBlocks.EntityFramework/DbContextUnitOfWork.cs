using System.Threading;
using System.Threading.Tasks;
using Enigmatry.Blueprint.BuildingBlocks.Core.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Blueprint.BuildingBlocks.EntityFramework
{
    [UsedImplicitly]
    public class DbContextUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly ILogger<DbContextUnitOfWork> _logger;
        private bool _cancelSaving;

        public DbContextUnitOfWork(DbContext context, ILogger<DbContextUnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public int SaveChanges()
        {
            var task = Task.Run(async () => await SaveChangesAsync());
            return task.Result;
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
