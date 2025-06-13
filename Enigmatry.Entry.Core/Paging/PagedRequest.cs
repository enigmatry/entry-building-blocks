﻿using MediatR;

namespace Enigmatry.Entry.Core.Paging;

public class PagedRequest<TResponse> : IRequest<PagedResponse<TResponse>>, IPagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = string.Empty;
    public string SortDirection { get; set; } = string.Empty;
}
