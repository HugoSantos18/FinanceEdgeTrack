using FinanceEdgeTrack.Domain.Entities;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Data;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class ReceitaRepository : Repository<Receita>, IReceitaRepository
{
    public ReceitaRepository(AppDbContext context) : base(context)
    {
    }
}
