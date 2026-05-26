using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinanceEdgeTrack.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> Query()
        => _context.Set<T>();

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        => await _context.Set<T>().FirstOrDefaultAsync(predicate);

    public async Task<T> CreateAsync(T entity)
    {
        try
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }
        catch (DbUpdateException dbEx)
        {
            var inner = dbEx.InnerException?.Message ?? dbEx.Message;
            throw new InvalidOperationException($"Erro ao salvar entidade do tipo {typeof(T).Name}: {inner}", dbEx);
        }
    }

    public Task UpdateAsync(T entity)
    {
        try
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }
        catch (DbUpdateException dbEx)
        {
            var inner = dbEx.InnerException?.Message ?? dbEx.Message;
            throw new InvalidOperationException($"Erro ao atualizar entidade do tipo {typeof(T).Name}: {inner}", dbEx);
        }
    }

    public Task DeleteAsync(T entity)
    {
        try
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }
        catch (DbUpdateException dbEx)
        {
            var inner = dbEx.InnerException?.Message ?? dbEx.Message;
            throw new InvalidOperationException($"Erro ao remover entidade do tipo {typeof(T).Name}: {inner}", dbEx);
        }
    }
}
