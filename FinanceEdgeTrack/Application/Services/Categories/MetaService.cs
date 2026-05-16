using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Pagination.Filters;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Enum;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using FinanceEdgeTrack.Domain.Interfaces.Services.Cache;
using FinanceEdgeTrack.Domain.Interfaces.Services.CarteiraService;
using FinanceEdgeTrack.Domain.Interfaces.Services.Categories;
using FinanceEdgeTrack.Domain.Models;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Application.Services.Categories;

public class MetaService : IMetaService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uof;
    private readonly ICarteiraService _carteiraService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<MetaService> _logger;
    private readonly ICacheService _cacheService;

    public MetaService(IMapper mapper, IUnitOfWork uof, ICarteiraService carteiraService,
                       ICurrentUser currentUser, ILogger<MetaService> logger, ICacheService cache)
    {
        _mapper = mapper;
        _uof = uof;
        _carteiraService = carteiraService;
        _currentUser = currentUser;
        _logger = logger;
        _cacheService = cache;
    }

    private string CacheKey()
    {
        return _cacheService.SetCacheKey(_currentUser.UserId);
    }

    public async Task<ApiResponse<MetaDTO>> GetMetaPorIdAsync(Guid metaId)
    {
        var meta = await _uof.MetaRepository
                             .Query()
                             .Where(m => m.Carteira != null && m.Carteira!.UserId == _currentUser.UserId)
                             .FirstOrDefaultAsync(m => m.MetaId == metaId);

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível encontrar meta de ID: {metaId}, verifique o ID informado.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> GetAllMetasAsync(PaginationParams pagination)
    {
        var query = _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira.UserId == _currentUser.UserId)
            .OrderByDescending(m => m.DataInicio)
            .AsNoTracking()
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaiorValorAsync(PaginationParams pagination)
    {
        var cached = await _cacheService.TryGetAsync<PagedList<MetaDTO>>(CacheKey());

        if (cached != null)
            return ApiResponse<PagedList<MetaDTO>>.Ok(cached);

        var query = _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId == _currentUser.UserId)
            .OrderByDescending(m => m.ValorAlvo)
            .AsNoTracking()
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        await _cacheService.SetAsync(CacheKey(), metasPaginadas, TimeSpan.FromMinutes(2));

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMenorValorAsync(PaginationParams pagination)
    {
        var cached = await _cacheService.TryGetAsync<PagedList<MetaDTO>>(CacheKey());

        if (cached != null)
            return ApiResponse<PagedList<MetaDTO>>.Ok(cached);

        var query = _uof.MetaRepository
            .Query()
            .AsNoTracking()
            .OrderBy(m => m.ValorAlvo)
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        await _cacheService.SetAsync(CacheKey(), metasPaginadas, TimeSpan.FromMinutes(2));

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasQuaseConcluidasAsync(PaginationParams pagination)
    {
        var cached = await _cacheService.TryGetAsync<PagedList<MetaDTO>>(CacheKey());

        if (cached != null)
            return ApiResponse<PagedList<MetaDTO>>.Ok(cached);

        var query = _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId == _currentUser.UserId)
            .OrderByDescending(m => m.ValorAtual)
            .AsNoTracking()
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        await _cacheService.SetAsync(CacheKey(), metasPaginadas, TimeSpan.FromMinutes(1));

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaisAntigaAsync(PaginationParams pagination)
    {
        var cached = await _cacheService.TryGetAsync<PagedList<MetaDTO>>(CacheKey());

        if (cached != null)
            return ApiResponse<PagedList<MetaDTO>>.Ok(cached);

        var query = _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId == _currentUser.UserId)
            .OrderBy(m => m.DataInicio)
            .AsNoTracking()
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        await _cacheService.SetAsync(CacheKey(), metasPaginadas, TimeSpan.FromMinutes(2));

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaisRecentesAsync(PaginationParams pagination)
    {
        var cached = await _cacheService.TryGetAsync<PagedList<MetaDTO>>(CacheKey());

        if (cached != null)
            return ApiResponse<PagedList<MetaDTO>>.Ok(cached);

        var query = _uof.MetaRepository
             .Query()
             .Where(m => m.Carteira != null && m.Carteira!.UserId == _currentUser.UserId)
             .OrderByDescending(m => m.DataInicio)
             .AsNoTracking()
             .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        await _cacheService.SetAsync(CacheKey(), metasPaginadas, TimeSpan.FromMinutes(2));

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasPorStatusAsync(StatusParams statusPagination)
    {
        var cached = await _cacheService.TryGetAsync<PagedList<MetaDTO>>(CacheKey());

        if (cached != null)
            return ApiResponse<PagedList<MetaDTO>>.Ok(cached);

        var query = _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId == _currentUser.UserId)
            .Where(m => m.Status == statusPagination.Status)
            .AsNoTracking()
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            statusPagination.PageNumber,
            statusPagination.PageSize
            );

        await _cacheService.SetAsync(CacheKey(), metasPaginadas, TimeSpan.FromMinutes(2));

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<MetaDTO>> CriarMetaAsync(CreateMetaDTO metaDto)
    {
        var carteira = await _carteiraService.GetCarteiraAsync();

        var meta = new Meta()
        {
            Titulo = metaDto.Titulo,
            Descricao = metaDto.Descricao,
            ValorAlvo = metaDto.ValorAlvo,
            DataInicio = metaDto.DataInicio,
            DataAlvo = metaDto.DataAlvo,
            CarteiraId = carteira.CarteiraId
        };

        await _uof.MetaRepository.CreateAsync(meta);
        await _uof.CommitAsync();

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

    public async Task<ApiResponse<MetaDTO>> AtualizarMetaAsync(Guid metaId, UpdateMetaDTO metaDto)
    {
        var meta = await _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId == _currentUser.UserId)
            .FirstOrDefaultAsync(m => m.MetaId == metaId);

        if (meta is null)
        {
            _logger.LogWarning($"Meta de ID {metaId} não encontrada, verifique o ID informado.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        meta.Titulo = metaDto.Titulo;
        meta.ValorAlvo = metaDto.ValorAlvo;
        meta.AlterarDataAlvo(metaDto.DataAlvo);
        meta.AlterarStatus(metaDto.Status);
        metaDto.UpdatedAt = DateTime.UtcNow;

        await _uof.MetaRepository.UpdateAsync(meta);
        await _uof.CommitAsync();

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta), $"Meta {meta.Titulo} atualizada com sucesso");
    }

    public async Task<ApiResponse<MetaDTO>> RemoverMetaAsync(Guid metaId)
    {
        var carteira = await _carteiraService.GetCarteiraAsync();

        var meta = await _uof.MetaRepository
                             .Query()
                             .Where(m => m.CarteiraId == carteira.CarteiraId)
                             .FirstOrDefaultAsync(m => m.MetaId == metaId);

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível remover meta, verifique o ID da meta {metaId} informado.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        carteira.Metas.Remove(meta);

        await _uof.CommitAsync();   

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

}
