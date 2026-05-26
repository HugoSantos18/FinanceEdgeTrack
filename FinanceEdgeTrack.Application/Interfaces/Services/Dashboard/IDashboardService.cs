using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Dashboard.Consolidado;

namespace FinanceEdgeTrack.Application.Interfaces.Services.Dashboard;

public interface IDashboardService
{
    Task<ApiResponse<DashboardConsolidadoDTO>> GetDashboardGeral();
    Task<ApiResponse<DashboardConsolidadoNoMesDTO>> GetDashboardMensal(int year, int month);
    Task<ApiResponse<DashboardConsolidadoPeriodoDTO>> GetDashboardPeriodo(DateTime start, DateTime end);

    // Posteriormente exportar para PDF
}
