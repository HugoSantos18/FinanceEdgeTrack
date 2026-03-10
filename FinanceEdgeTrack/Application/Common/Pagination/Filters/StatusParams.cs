using FinanceEdgeTrack.Domain.Enum;

namespace FinanceEdgeTrack.Application.Common.Pagination.Filters;

public class StatusParams : PaginationParams
{
  public Status? Status { get; set; }
}
