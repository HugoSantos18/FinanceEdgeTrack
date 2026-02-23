using FinanceEdgeTrack.Domain.Interfaces;

namespace FinanceEdgeTrack.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    // instancia da interface de cada repositorio para utilizar.


    public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger)
    {
        this._context = context;
        this._logger = logger;
    }



    public async void Commit()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
