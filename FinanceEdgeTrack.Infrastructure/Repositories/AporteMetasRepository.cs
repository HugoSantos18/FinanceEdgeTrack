using FinanceEdgeTrack.Domain.Entities;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Data;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class AporteMetasRepository : Repository<AporteMetas>, IAporteMetasRepository
{
    public AporteMetasRepository(AppDbContext context) : base(context)
    {
    }
}
