using FinanceEdgeTrack.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace FinanceEdgeTrack.Infrastructure.Transactions;

internal sealed class EfCoreTransaction(IDbContextTransaction inner) : ITransaction
{
    public Task CommitAsync(CancellationToken ct = default) => inner.CommitAsync(ct);
    public Task RollbackAsync(CancellationToken ct = default) => inner.RollbackAsync(ct);
    public ValueTask DisposeAsync() => inner.DisposeAsync();
}
