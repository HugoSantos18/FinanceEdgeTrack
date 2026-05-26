using FinanceEdgeTrack.Domain.Enums;

namespace FinanceEdgeTrack.Application.Common.Pagination;

public class StatusParams : PaginationParams
{
    public Status? Status { get; set; }
}
