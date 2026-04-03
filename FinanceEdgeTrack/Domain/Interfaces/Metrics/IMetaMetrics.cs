using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Metas;

namespace FinanceEdgeTrack.Domain.Interfaces.Metrics;

public interface IMetaMetrics
{
    Task<ApiResponse<MetasResumoMensalDTO>> GetMetricsMetasNoMes(int month);
    Task<ApiResponse<MetasResumoGeralDTO>> GetMetricsMetas();
    Task<ApiResponse<MetasKPIsNoMesDTO>> GetKPIsMetasNoMes(int month);
    Task<ApiResponse<MetasKPIsGeralDTO>> GetKPIsMetas();
}
