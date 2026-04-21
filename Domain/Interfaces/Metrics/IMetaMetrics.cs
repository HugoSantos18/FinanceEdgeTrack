using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Metas;

namespace FinanceEdgeTrack.Domain.Interfaces.Metrics;

public interface IMetaMetrics
{
    Task<ApiResponse<MetasResumoMensalDTO>> GetMetricsMetasNoMes(int year, int month);
    Task<ApiResponse<MetasResumoGeralDTO>> GetMetricsMetas();
    Task<ApiResponse<MetasResumoPeriodoDTO>> GetMetricsMetasNoPeriodo(DateTime start, DateTime end);
    Task<ApiResponse<MetasKPIsNoMesDTO>> GetKPIsMetasNoMes(int year, int month);
    Task<ApiResponse<MetasKPIsPeriodoDTO>> GetKPIsMetasNoPeriodo(DateTime start, DateTime end);
    Task<ApiResponse<MetasKPIsGeralDTO>> GetKPIsMetas();
}
