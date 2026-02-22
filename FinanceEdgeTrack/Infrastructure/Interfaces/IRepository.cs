using System.Linq.Expressions;

namespace FinanceEdgeTrack.Infrastructure.Interfaces;

public interface IRepository<T>
{
    Task<T>? Get(Expression<Func<T, bool>> predicade);

    IEnumerable<T>? GetAll();

    T Create(T entity);

    T? Update(T entity);

    T? Delete(T entity);

}
