using FinanceEdgeTrack.Domain.Models.Abstract;
using FinanceEdgeTrack.Infrastructure.Data;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {

    }

}
