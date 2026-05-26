using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Dashboard.Carteira;

namespace FinanceEdgeTrack.Application.Interfaces.Services.Metrics;

public interface ICarteiraMetrics
{
    Task<ApiResponse<CarteiraResumoDTO>> GetSaldoAtualUser();
}
