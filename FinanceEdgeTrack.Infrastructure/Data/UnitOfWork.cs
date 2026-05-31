using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Repositories;
using FinanceEdgeTrack.Infrastructure.Transactions;

namespace FinanceEdgeTrack.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private ICarteiraRepository? _carteiraRepository;
    private IDespesaRepository? _despesaRepository;
    private IReceitaRepository? _receitaRepository;
    private IMetaRepository? _metaRepository;
    private IAporteMetasRepository? _aporteMetasRepository;


    public IDespesaRepository DespesaRepository
    {
        get
        {
            return _despesaRepository = _despesaRepository ?? new DespesaRepository(_context);
        }
    }

    public IReceitaRepository ReceitaRepository
    {
        get
        {
            return _receitaRepository = _receitaRepository ?? new ReceitaRepository(_context);
        }
    }

    public IMetaRepository MetaRepository
    {
        get
        {
            return _metaRepository = _metaRepository ?? new MetaRepository(_context);
        }
    }


    public ICarteiraRepository CarteiraRepository
    {
        get
        {
            return _carteiraRepository = _carteiraRepository ?? new CarteiraRepository(_context);
        }
    }

    public IAporteMetasRepository AporteMetasRepository
    {
        get
        {
            return _aporteMetasRepository = _aporteMetasRepository ?? new AporteMetasRepository(_context);
        }
    }

    public UnitOfWork(AppDbContext context)
    {
        this._context = context;
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        return new EfCoreTransaction(await _context.Database.BeginTransactionAsync());
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
