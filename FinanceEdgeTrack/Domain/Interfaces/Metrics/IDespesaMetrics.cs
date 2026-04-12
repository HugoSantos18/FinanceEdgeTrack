using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Despesas;

namespace FinanceEdgeTrack.Domain.Interfaces.Metrics;

public interface IDespesaMetrics
{
    Task<ApiResponse<DespesasResumoMensalDTO>> GetDespesaMetricsNoMes(int year, int month);
    Task<ApiResponse<DespesasResumoPeriodoDTO>> GetDespesaMetricsNoPeriodo(DateTime start, DateTime end);
    Task<ApiResponse<DespesasGeralDTO>> GetDespesaMetricsTotal();
}
