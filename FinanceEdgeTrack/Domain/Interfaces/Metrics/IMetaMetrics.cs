using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Metas;

namespace FinanceEdgeTrack.Domain.Interfaces.Metrics;

public interface IMetaMetrics
{
    Task<ApiResponse<PagedList<MetaDetalhadaResumoDTO>>> Metas(PaginationParams pagination); // verificar como usar
    Task<ApiResponse<MetasResumoMensalDTO>> GetMetricsMetasNoMes();
    Task<ApiResponse<MetasResumoGeralDTO>> GetMetricsMetas();
    Task<ApiResponse<MetasKPIsDTO>> GetKPIsMetas();
}
