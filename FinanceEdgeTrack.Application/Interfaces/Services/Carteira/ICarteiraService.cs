using CarteiraEntity = FinanceEdgeTrack.Domain.Entities.Carteira;

namespace FinanceEdgeTrack.Application.Interfaces.Services.Carteira;

public interface ICarteiraService
{
    Task<CarteiraEntity> CreateAsync(string userId);
    Task<CarteiraEntity> GetCarteiraAsync();
    Task<bool> DebitarSaldoComGuardaAsync(Guid carteiraId, decimal valor);
    Task CreditarSaldoAsync(Guid carteiraId, decimal valor);
}
