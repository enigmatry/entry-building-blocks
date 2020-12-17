using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Enigmatry.Blueprint.BuildingBlocks.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.EntityFramework
{
    public static class EntityQueryableExtensions
    {
        public static async Task<T> SingleOrNotFoundAsync<T>(this IQueryable<T> query,
            CancellationToken cancellationToken = default)
        {
            var result = await query.SingleOrDefaultAsync(cancellationToken);
            return result != null ? result : throw new EntityNotFoundException(typeof(T).Name);
        }

        public static async Task<TDestination> SingleOrDefaultMappedAsync<TSource, TDestination>(
            this IQueryable<TSource> query, IMapper mapper, CancellationToken cancellationToken = default)
        {
            var item = await query.SingleOrDefaultAsync(cancellationToken);
            return mapper.Map<TDestination>(item);
        }

        public static async Task<List<TDestination>> ToListMappedAsync<TSource, TDestination>(
            this IQueryable<TSource> query, IMapper mapper, CancellationToken cancellationToken = default)
        {
            var items = await query.ToListAsync(cancellationToken);
            return mapper.Map<List<TDestination>>(items);
        }
    }
}
