namespace FinanceEdgeTrack.Application.Common.Pagination;

public class PaginationParams
{
    private const int MaxPageSize = 40;
    public int PageNumber { get; set; }

    private int _pageSize = 10;

    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
