using FinanceEdgeTrack.Domain.Models;

namespace FinanceEdgeTrack.Domain.Interfaces.Repositories;

public interface IMetaRepository : IRepository<Meta>
{
    // Metas com filtros para: Maior aporte, ultima modificação, pagination.

    // ex: PagedList<Meta> GetMetasPorMaiorAporte();
}
