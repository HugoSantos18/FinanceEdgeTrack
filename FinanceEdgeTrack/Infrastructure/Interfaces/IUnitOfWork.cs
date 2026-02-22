using FinanceEdgeTrack.Infrastructure.Repositories;

namespace FinanceEdgeTrack.Infrastructure.Interfaces;

public interface IUnitOfWork
{
    // Propriedades dos repositories com get somente.


    void Commit();
    void Dispose();
}
