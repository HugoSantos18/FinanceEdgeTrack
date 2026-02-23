using FinanceEdgeTrack.Infrastructure.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinanceEdgeTrack.Domain.Interfaces;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    private readonly IUnitOfWork _uof;

    public Repository(AppDbContext context, IUnitOfWork uof)
    {
        this._context = context;
        this._uof = uof;
    }

    public T Create(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));


        _context.Set<T>().Add(entity);

        _uof.Commit();

        return entity;
    }

    public T Delete(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        _context.Set<T>().Remove(entity);

        _uof.Commit();

        return entity;
    }
    public T Update(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        _context.Entry(entity).State = EntityState.Modified;

        _uof.Commit();

        return entity;
    }

    public async Task<T> Get(Expression<Func<T, bool>> predicade)
    {
        var entity = await _context.Set<T>().FindAsync(predicade);

        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        return entity;
    }

    public IEnumerable<T> GetAll()
    {
        return _context.Set<T>().ToList();
    }


}


