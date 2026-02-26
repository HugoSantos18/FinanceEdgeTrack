using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Infrastructure.Data;
namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class CarteiraRepository : Repository<Carteira>, ICarteiraRepository
{
    public CarteiraRepository(AppDbContext context) : base(context)
    {
    }

}
