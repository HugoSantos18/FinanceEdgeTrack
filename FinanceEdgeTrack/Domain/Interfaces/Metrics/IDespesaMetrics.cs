using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Despesas;

namespace FinanceEdgeTrack.Domain.Interfaces.Metrics;

public interface IDespesaMetrics
{
    Task<ApiResponse<DespesasResumoMensalDTO>> GetDespesaMetricsNoMes(int month);
    Task<ApiResponse<DespesasGeralDTO>> GetDespesaMetricsTotal();
}
