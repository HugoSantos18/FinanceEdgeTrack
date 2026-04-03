using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Consolidado;

namespace FinanceEdgeTrack.Domain.Interfaces.Services.Dashboard;

public interface IDashboardService
{
    Task<ApiResponse<DashboardConsolidadoDTO>> GetDashboardGeral();
    Task<ApiResponse<DashboardConsolidadoNoMesDTO>> GetDashboardMensal(int year, int month);
    Task<ApiResponse<DashboardConsolidadoPeriodoDTO>> GetDashboardPeriodo(DateTime start, DateTime end);

    // Escrever em algum arquivo talvez 
}
