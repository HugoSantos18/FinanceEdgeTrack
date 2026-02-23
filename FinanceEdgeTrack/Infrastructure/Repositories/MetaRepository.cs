using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Infrastructure.Data;
using System.Linq.Expressions;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class MetaRepository : Repository<Meta>, IMetaRepository
{
    public MetaRepository(AppDbContext context) : base(context)
    {

    }


}
