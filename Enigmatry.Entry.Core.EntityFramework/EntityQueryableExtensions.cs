using System.Linq.Dynamic.Core;
using Enigmatry.Entry.Core.Entities;
using Enigmatry.Entry.Core.Paging;
using Microsoft.EntityFrameworkCore;

namespace Enigmatry.Entry.Core.EntityFramework;

public static class EntityQueryableExtensions
{
    extension<T>(IQueryable<T> query)
    {
        public async Task<T> SingleOrNotFoundAsync(CancellationToken cancellationToken = default)
        {
            var result = await query.SingleOrDefaultAsync(cancellationToken);
            return result ?? throw new EntityNotFoundException(typeof(T).Name);
        }

        public PagedResponse<T> ToPagedResponse(IPagedRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var pagedQuery = query
                .OrderByDynamic(request.SortBy, request.SortDirection);

            var skipPaging = request.PageSize == Int32.MaxValue;

            if (!skipPaging)
            {
                pagedQuery = pagedQuery.Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);
            }

            var items = pagedQuery.ToList();

            var totalCount = skipPaging ? items.Count : query.Count();

            return new PagedResponse<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber
            };
        }

        public async Task<PagedResponse<T>> ToPagedResponseAsync(IPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var pagedQuery = query
                .OrderByDynamic(request.SortBy, request.SortDirection);

            var skipPaging = request.PageSize == Int32.MaxValue;

            if (!skipPaging)
            {
                pagedQuery = pagedQuery.Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);
            }

            var items = await pagedQuery.ToListAsync(cancellationToken);

            var totalCount = skipPaging ? items.Count : await query.CountAsync(cancellationToken);

            return new PagedResponse<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber
            };
        }

        private IQueryable<T> OrderByDynamic(string? orderBy, string orderDirection = "asc") =>
            string.IsNullOrWhiteSpace(orderBy) ? query : query.OrderBy($"{orderBy} {orderDirection}");
    }
}
