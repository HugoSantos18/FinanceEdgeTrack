using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Repositories;

namespace FinanceEdgeTrack.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    // variáveis para armazenar os repositórios instanciados.
    private ICarteiraRepository? _carteiraRepository;
    private IDespesaRepository? _despesaRepository;
    private IReceitaRepository? _receitaRepository;
    private IMetaRepository? _metaRepository;
    private ILancamentoRepository? _lancamentoRepository;


    // instancia da interface de cada repositorio para utilizar.
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

    public ILancamentoRepository LancamentoRepository
    {
        get
        {
            return _lancamentoRepository = _lancamentoRepository ?? new LancamentoRepository(_context);
        }
    }

    public ICarteiraRepository CarteiraRepository
    {
        get
        {
            return _carteiraRepository = _carteiraRepository ?? new CarteiraRepository(_context);
        }
    }

    public UnitOfWork(AppDbContext context)
    {
        this._context = context;
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
