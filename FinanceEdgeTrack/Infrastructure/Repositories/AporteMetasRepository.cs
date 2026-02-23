using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Infrastructure.Data;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class AporteMetasRepository : Repository<AporteMetas>, IAporteMetasRepository
{
    public AporteMetasRepository(AppDbContext context) : base(context)
    {

    }


}
