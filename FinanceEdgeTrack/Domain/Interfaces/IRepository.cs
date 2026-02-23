using System.Linq.Expressions;

namespace FinanceEdgeTrack.Domain.Interfaces;

public interface IRepository<T>
{
    Task<T>? Get(Expression<Func<T, bool>> predicade);

    Task<IEnumerable<T>>? GetAll();

    Task<T> Create(T entity);

    Task<T>? Update(T entity);

    Task<T>? Delete(T entity);

}
