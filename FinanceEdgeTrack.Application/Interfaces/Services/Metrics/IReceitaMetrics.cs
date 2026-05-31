using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Dashboard.Receitas;

namespace FinanceEdgeTrack.Application.Interfaces.Services.Metrics;

public interface IReceitaMetrics
{
    Task<ApiResponse<ReceitasResumoMensalDTO>> GetReceitaMetricsNoMes(int year, int month);
    Task<ApiResponse<ReceitasGeralDTO>> GetReceitaMetrics();
    Task<ApiResponse<ReceitasResumoPeriodoDTO>> GetReceitaMetricsNoPeriodo(DateTime start, DateTime end);
}
