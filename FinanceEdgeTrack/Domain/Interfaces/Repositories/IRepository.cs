using System.Linq.Expressions;

namespace FinanceEdgeTrack.Domain.Interfaces.Repositories;

public interface IRepository<T>
{
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    IQueryable<T> GetAll();
    IQueryable<T> Query();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);

}
