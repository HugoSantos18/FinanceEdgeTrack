using FinanceEdgeTrack.Domain.Entities;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Data;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class DespesaRepository : Repository<Despesa>, IDespesaRepository
{
    public DespesaRepository(AppDbContext context) : base(context)
    {
    }
}
