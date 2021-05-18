using System;
using MediatR;

namespace Enigmatry.BuildingBlocks.Core.Paging
{
    public class PagedRequest<TResponse> : IRequest<PagedResponse<TResponse>>, IPagedRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = String.Empty;
        public string SortDirection { get; set; } = String.Empty;
    }
}
