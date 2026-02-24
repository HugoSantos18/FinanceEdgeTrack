using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Infrastructure.Repositories;

namespace FinanceEdgeTrack.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    // variáveis para armazenar os repositórios instanciados.
    private ICategoriaRepository? _categoriaRepository;
    private IAporteMetasRepository? _aporteMetasRepository;
    private IMetaRepository? _metaRepository;
    private ILancamentoRepository? _lancamentoRepository;


    // instancia da interface de cada repositorio para utilizar.
    public ICategoriaRepository CategoriaRepository
    {
        get
        {
            return _categoriaRepository = _categoriaRepository ?? new CategoriaRepository(_context);
        }
    }

    public IAporteMetasRepository AporteMetasRepository
    {
        get
        {
            return _aporteMetasRepository = _aporteMetasRepository ?? new AporteMetasRepository(_context);
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


    public UnitOfWork(AppDbContext context)
    {
        this._context = context;
    }

    public async void CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
