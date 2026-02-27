using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Infrastructure.Repositories;

namespace FinanceEdgeTrack.Domain.Interfaces.Repositories;

public interface ILancamentoRepository : IRepository<Lancamento>
{

    // métodos com filtros e paginação posteriormente.
}
