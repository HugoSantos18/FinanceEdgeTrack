using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Repositories;

namespace FinanceEdgeTrack.Domain.Interfaces;

public interface IUnitOfWork
{
    // Propriedades dos repositories com get somente.
    ICarteiraRepository CarteiraRepository { get; }
    IReceitaRepository ReceitaRepository { get; }
    IDespesaRepository DespesaRepository { get; }
    IMetaRepository MetaRepository { get; }

    Task CommitAsync();
    void Dispose();
}
