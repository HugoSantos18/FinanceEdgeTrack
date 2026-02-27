using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Infrastructure.Data;

namespace FinanceEdgeTrack.Infrastructure.Repositories;


public class LancamentoRepository : Repository<Lancamento>, ILancamentoRepository
{
    public LancamentoRepository(AppDbContext context) : base(context)
    {

    } 

}
