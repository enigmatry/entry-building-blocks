namespace Enigmatry.Entry.Core.Paging;

public interface IPagedRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string SortBy { get; set; }
    public string SortDirection { get; set; }
}
