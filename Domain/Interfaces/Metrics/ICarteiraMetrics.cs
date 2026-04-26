using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Carteira;

namespace FinanceEdgeTrack.Domain.Interfaces.Metrics;

public interface ICarteiraMetrics
{
    Task<ApiResponse<CarteiraResumoDTO>> GetSaldoAtualUser();
}
