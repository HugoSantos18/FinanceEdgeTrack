using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Repositories;

namespace FinanceEdgeTrack.Domain.Interfaces;

public interface IUnitOfWork
{
    // Propriedades dos repositories com get somente.
    ICategoriaRepository CategoriaRepository { get; }
    IAporteMetasRepository AporteMetasRepository { get; }
    ILancamentoRepository LancamentoRepository { get; }
    IMetaRepository MetaRepository { get; }

    void CommitAsync();
    void Dispose();
}
