using FinanceEdgeTrack.Domain.Entities;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Data;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class MetaRepository : Repository<Meta>, IMetaRepository
{
    public MetaRepository(AppDbContext context) : base(context)
    {
    }
}
