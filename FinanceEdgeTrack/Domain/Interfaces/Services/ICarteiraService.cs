using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Application.Dtos.Write.Carteira;
using FinanceEdgeTrack.Domain.Models;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface ICarteiraService
{
    Task<Carteira> CreateAsync(CreateCarteiraDTO carteiraDto);
    Task AdicionarSaldoAsync(string userId, decimal valor);
    Task DescontarSaldoAsync(string userId,decimal valor);
    Task<decimal> ObterSaldoAsync(string userId);
}
