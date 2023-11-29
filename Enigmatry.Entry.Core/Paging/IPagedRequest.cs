namespace Enigmatry.Entry.Core.Paging
{
    public interface IPagedRequest
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }
        string? SortBy { get; set; }
        string? SortDirection { get; set; }
    }
}
