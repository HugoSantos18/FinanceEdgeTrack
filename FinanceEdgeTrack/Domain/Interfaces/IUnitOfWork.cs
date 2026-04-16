using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FinanceEdgeTrack.Domain.Interfaces;

public interface IUnitOfWork
{
    ICarteiraRepository CarteiraRepository { get; }
    IReceitaRepository ReceitaRepository { get; }
    IDespesaRepository DespesaRepository { get; }
    IMetaRepository MetaRepository { get; }
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitAsync();
    void Dispose();
}
