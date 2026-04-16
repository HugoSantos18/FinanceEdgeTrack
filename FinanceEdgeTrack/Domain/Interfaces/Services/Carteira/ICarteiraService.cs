using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Domain.Models;

namespace FinanceEdgeTrack.Domain.Interfaces.Services.CarteiraService;

public interface ICarteiraService
{
    Task<Carteira> CreateAsync();
    Task<Carteira> GetCarteiraAsync();
    Task <ApiResponse<decimal>> AdicionarSaldoAsync(decimal valor);
    Task <ApiResponse<decimal>>DescontarSaldoAsync(decimal valor);
    Task<ApiResponse<decimal>> ObterSaldoAsync();
}
