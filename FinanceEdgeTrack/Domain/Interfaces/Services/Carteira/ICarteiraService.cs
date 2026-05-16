using FinanceEdgeTrack.Domain.Models;

namespace FinanceEdgeTrack.Domain.Interfaces.Services.CarteiraService;

public interface ICarteiraService
{
    Task<Carteira> CreateAsync(string userId);
    Task<Carteira> GetCarteiraAsync();
    Task<bool> DebitarSaldoComGuardaAsync(Guid carteiraId, decimal valor);
    Task CreditarSaldoAsync(Guid carteiraId, decimal valor);
}
