using FinanceEdgeTrack.Domain.Models.Abstract;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Infrastructure.Data;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {

    }

}
