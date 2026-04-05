using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Receitas;

namespace FinanceEdgeTrack.Domain.Interfaces.Metrics;

public interface IReceitaMetrics
{
    Task<ApiResponse<ReceitasResumoMensalDTO>> GetReceitaMetricsNoMes(int month);
    Task<ApiResponse<ReceitasGeralDTO>> GetReceitaMetrics();
    Task<ApiResponse<ReceitasResumoPeriodoDTO>> GetReceitaMetricsNoPeriodo(DateTime start, DateTime end);
}
