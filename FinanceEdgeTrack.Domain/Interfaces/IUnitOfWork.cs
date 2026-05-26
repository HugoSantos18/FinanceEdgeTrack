using FinanceEdgeTrack.Domain.Interfaces.Repositories;

namespace FinanceEdgeTrack.Domain.Interfaces;

public interface IUnitOfWork
{
    ICarteiraRepository CarteiraRepository { get; }
    IReceitaRepository ReceitaRepository { get; }
    IDespesaRepository DespesaRepository { get; }
    IMetaRepository MetaRepository { get; }
    IAporteMetasRepository AporteMetasRepository { get; }
    Task<ITransaction> BeginTransactionAsync();
    Task CommitAsync();
    void Dispose();
}
