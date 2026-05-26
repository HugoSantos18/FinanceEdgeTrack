using FinanceEdgeTrack.Domain.Entities;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class CarteiraRepository : Repository<Carteira>, ICarteiraRepository
{
    public CarteiraRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> DebitarSaldoComGuardaAsync(Guid carteiraId, decimal valor)
    {
        var rows = await _context.Set<Carteira>()
            .Where(c => c.CarteiraId == carteiraId && c.Saldo >= valor)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Saldo, c => c.Saldo - valor));

        return rows == 1;
    }

    public async Task CreditarSaldoAsync(Guid carteiraId, decimal valor)
    {
        await _context.Set<Carteira>()
            .Where(c => c.CarteiraId == carteiraId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Saldo, c => c.Saldo + valor));
    }
}
