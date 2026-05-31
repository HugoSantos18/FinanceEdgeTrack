using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Metas;
using FinanceEdgeTrack.Application.DTOs.Write.Categorias;
using FinanceEdgeTrack.Domain.Enums;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Application.Interfaces.Services.Auth;
using FinanceEdgeTrack.Application.Interfaces.Services.Carteira;
using FinanceEdgeTrack.Application.Interfaces.Services.Categories;
using FinanceEdgeTrack.Domain.Entities;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Application.Services.Categories;

public class AporteMetasService : IAporteMetasService
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;
    private readonly ICarteiraService _carteiraService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AporteMetasService> _logger;

    public AporteMetasService(IUnitOfWork uof, IMapper mapper, ICarteiraService carteiraService,
                              ICurrentUser currentUser, ILogger<AporteMetasService> logger)
    {
        _uof = uof;
        _mapper = mapper;
        _carteiraService = carteiraService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ApiResponse<AporteMetasDTO>> GetAporteByIdAsync(Guid aporteMetaId)
    {
        var aporte = await _uof.AporteMetasRepository
            .Query()
            .Where(a => a.AporteMetasId == aporteMetaId)
            .Where(a => _uof.MetaRepository.Query()
                .Any(m => m.MetaId == a.MetaId
                       && m.Carteira != null
                       && m.Carteira!.UserId == _currentUser.UserId))
            .AsNoTracking()
            .ProjectToType<AporteMetasDTO>()
            .FirstOrDefaultAsync();

        if (aporte is null)
        {
            _logger.LogInformation($"Não foi possível encontrar aporte de ID: {aporteMetaId}.");
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundAporte);
        }

        return ApiResponse<AporteMetasDTO>.Ok(aporte);
    }

    public async Task<ApiResponse<PagedList<AporteMetasDTO>>> GetAportesDaMetaAsync(Guid metaId, PaginationParams pagination)
    {
        var metaPertenceAoUser = await _uof.MetaRepository
            .Query()
            .AnyAsync(m => m.MetaId == metaId
                        && m.Carteira != null
                        && m.Carteira!.UserId == _currentUser.UserId);

        if (!metaPertenceAoUser)
            return ApiResponse<PagedList<AporteMetasDTO>>.Fail(ResultMessages.NotFoundMeta);

        var query = _uof.AporteMetasRepository
            .Query()
            .Where(a => a.MetaId == metaId)
            .OrderByDescending(a => a.Data)
            .AsNoTracking()
            .ProjectToType<AporteMetasDTO>();

        var aportesPaginados = await PagedList<AporteMetasDTO>.CreateAsync(
            query,
            pagination.PageNumber,
            pagination.PageSize);

        return ApiResponse<PagedList<AporteMetasDTO>>.Ok(aportesPaginados);
    }

    public async Task<ApiResponse<decimal>> ValorTotalDaMetaAsync(Guid metaId)
    {
        var metaPertenceAoUser = await _uof.MetaRepository
            .Query()
            .AnyAsync(m => m.MetaId == metaId
                        && m.Carteira != null
                        && m.Carteira!.UserId == _currentUser.UserId);

        if (!metaPertenceAoUser)
            return ApiResponse<decimal>.Fail(ResultMessages.NotFoundMeta);

        var total = await _uof.AporteMetasRepository
            .Query()
            .Where(a => a.MetaId == metaId)
            .SumAsync(a => (decimal?)a.Valor) ?? 0;

        return ApiResponse<decimal>.Ok(total, $"Valor total investido na meta {total:C2}");
    }

    public async Task<ApiResponse<AporteMetasDTO>> RegistrarAporteAsync(Guid metaId, CreateAporteMetaDTO dto)
    {
        if (dto.Valor <= 0)
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.MoreThanZero);

        var meta = await _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId == _currentUser.UserId)
            .FirstOrDefaultAsync(m => m.MetaId == metaId);

        if (meta is null)
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundMeta);

        await using var tx = await _uof.BeginTransactionAsync();

        var debitou = await _carteiraService.DebitarSaldoComGuardaAsync(meta.CarteiraId, dto.Valor);
        if (!debitou)
        {
            await tx.RollbackAsync();
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.InsufficientBalance);
        }

        var aporte = AporteMetas.Criar(meta.MetaId, dto.Valor);
        await _uof.AporteMetasRepository.CreateAsync(aporte);
        await _uof.CommitAsync();

        var agg = await _uof.AporteMetasRepository   // agregate for update results.
            .Query()
            .Where(a => a.MetaId == metaId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Sum(a => a.Valor),
                UltimoValor = g.OrderByDescending(a => a.Data).Select(a => a.Valor).First(),
                UltimaData = g.OrderByDescending(a => a.Data).Select(a => a.Data).First()
            })
            .FirstAsync();

        meta.RecalcularProgresso(agg.Total, agg.UltimoValor, agg.UltimaData);
        await _uof.CommitAsync();
        await tx.CommitAsync();

        var aporteDto = _mapper.Map<AporteMetasDTO>(aporte);
        var msg = meta.Status == Status.Concluido
            ? $"🎉 Meta concluída em {(meta.DataConclusao!.Value - meta.DataInicio).Days} dias!"
            : $"Aporte de {dto.Valor:C2} registrado.";

        _logger.LogInformation($"Aporte {aporte.AporteMetasId} registrado na meta {metaId} (carteira {meta.CarteiraId}, valor {dto.Valor:C2}).");

        return ApiResponse<AporteMetasDTO>.Ok(aporteDto, msg);
    }

    public async Task<ApiResponse<AporteMetasDTO>> RemoverAporteAsync(Guid aporteMetaId)
    {
        var aporte = await _uof.AporteMetasRepository
            .Query()
            .FirstOrDefaultAsync(a => a.AporteMetasId == aporteMetaId);

        if (aporte is null)
        {
            _logger.LogWarning($"Aporte {aporteMetaId} não encontrado.");
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundAporte);
        }

        var meta = await _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId == _currentUser.UserId)
            .FirstOrDefaultAsync(m => m.MetaId == aporte.MetaId);

        if (meta is null)
        {
            _logger.LogWarning($"Meta {aporte.MetaId} do aporte {aporteMetaId} não pertence ao usuário corrente.");
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        var valorRemovido = aporte.Valor;
        var aporteDto = _mapper.Map<AporteMetasDTO>(aporte);

        await using var tx = await _uof.BeginTransactionAsync();

        await _uof.AporteMetasRepository.DeleteAsync(aporte);
        await _uof.CommitAsync();

        await _carteiraService.CreditarSaldoAsync(meta.CarteiraId, valorRemovido);

        var agg = await _uof.AporteMetasRepository
            .Query()
            .Where(a => a.MetaId == meta.MetaId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Sum(a => a.Valor),
                UltimoValor = g.OrderByDescending(a => a.Data).Select(a => a.Valor).FirstOrDefault(),
                UltimaData = g.OrderByDescending(a => a.Data).Select(a => a.Data).FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        var total = agg?.Total ?? 0m;
        var ultimoValor = agg?.UltimoValor ?? 0m;
        var ultimaData = agg?.UltimaData ?? default;

        meta.RecalcularProgresso(total, ultimoValor, ultimaData);
        await _uof.CommitAsync();
        await tx.CommitAsync();

        _logger.LogInformation($"Aporte {aporteMetaId} removido da meta {meta.MetaId}; saldo creditado em {valorRemovido:C2}.");

        return ApiResponse<AporteMetasDTO>.Ok(aporteDto, $"Aporte no valor de {valorRemovido:C2} removido com sucesso.");
    }
}
