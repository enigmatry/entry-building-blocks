using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Enigmatry.Entry.Core.Entities;
using Enigmatry.Entry.Core.Paging;
using Microsoft.EntityFrameworkCore;

namespace Enigmatry.Entry.Core.EntityFramework;

public static class EntityQueryableExtensions
{
    public static async Task<T> SingleOrNotFoundAsync<T>(this IQueryable<T> query,
        CancellationToken cancellationToken = default)
    {
        var result = await query.SingleOrDefaultAsync(cancellationToken);
        return result ?? throw new EntityNotFoundException(typeof(T).Name);
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

    public static PagedResponse<T> ToPagedResponse<T>(this IQueryable<T> query, IPagedRequest request)
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

    public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(this IQueryable<T> query, IPagedRequest request,
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

    private static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string? orderBy, string orderDirection = "asc") =>
        string.IsNullOrWhiteSpace(orderBy) ? query : query.OrderBy($"{orderBy} {orderDirection}");
}
