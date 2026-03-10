namespace FinanceEdgeTrack.Application.Common.Pagination;

public interface IPagedList
{
    int PageNumber { get; }
    int PageSize { get; }
    int TotalCount { get; }
    int TotalPages { get; }
    bool HasNext { get; }
    bool HasPrevious { get; }

}
