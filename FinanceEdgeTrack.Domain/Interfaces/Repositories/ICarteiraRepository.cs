using FinanceEdgeTrack.Domain.Entities;

namespace FinanceEdgeTrack.Domain.Interfaces.Repositories;

public interface ICarteiraRepository : IRepository<Carteira>
{
    Task<bool> DebitarSaldoComGuardaAsync(Guid carteiraId, decimal valor);
    Task CreditarSaldoAsync(Guid carteiraId, decimal valor);
}
