using FinanceEdgeTrack.Application.Dtos.Read;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface ICarteiraService
{
    Task AdicionarSaldoAsync(string userId, decimal valor);
    Task DescontarSaldoAsync(string userId,decimal valor);
    Task<decimal> ObterSaldoAsync(string userId);
}
