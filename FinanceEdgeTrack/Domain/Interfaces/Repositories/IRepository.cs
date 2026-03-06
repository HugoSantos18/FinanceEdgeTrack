using System.Linq.Expressions;

namespace FinanceEdgeTrack.Domain.Interfaces.Repositories;

public interface IRepository<T>
{
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);

}
