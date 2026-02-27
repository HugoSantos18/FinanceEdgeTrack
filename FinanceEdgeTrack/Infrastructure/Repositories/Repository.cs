using FinanceEdgeTrack.Infrastructure.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        this._context = context;
    }

    public async Task<T> Create(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));


        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<T> Delete(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();

        return entity;
    }
    public async Task<T> Update(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<T> Get(Expression<Func<T, bool>> predicade)
    {
        var entity = await _context.Set<T>().FindAsync(predicade);

        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        return entity;
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        return await _context.Set<T>().ToListAsync();
    }


}


