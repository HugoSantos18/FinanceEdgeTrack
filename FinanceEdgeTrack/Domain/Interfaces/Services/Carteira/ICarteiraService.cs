using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Application.Dtos.Write.Carteira;
using FinanceEdgeTrack.Domain.Models;

namespace FinanceEdgeTrack.Domain.Interfaces.Services.Carteira;

public interface ICarteiraService
{
    Task<Carteira> CreateAsync(CreateCarteiraDTO carteiraDto);
    Task <ApiResponse<decimal>> AdicionarSaldoAsync(string userId, decimal valor);
    Task <ApiResponse<decimal>>DescontarSaldoAsync(string userId,decimal valor);
    Task<ApiResponse<decimal>> ObterSaldoAsync(string userId);
}
