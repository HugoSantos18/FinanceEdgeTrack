using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
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

public class ReceitaService : IReceitaService
{

    private readonly IUnitOfWork _uof;
    private readonly ICarteiraService _carteiraService;
    private readonly IMapper _mapper;
    private readonly ILogger<ReceitaService> _logger;
    private readonly ICurrentUser _currentUser;
    private readonly ICacheService _cacheService;

    public ReceitaService(IUnitOfWork uof, IMapper mapper, ICarteiraService carteira,
                          ILogger<ReceitaService> logger, ICurrentUser currentUser, ICacheService cache)
    {
        _mapper = mapper;
        _uof = uof;
        _carteiraService = carteira;
        _logger = logger;
        _currentUser = currentUser;
        _cacheService = cache;
    }

    private string CacheKey()
    {
        return _cacheService.SetCacheKey(_currentUser.UserId);
    }

    public async Task<ApiResponse<ReceitaDTO>> ObterReceitaPorIdAsync(Guid id)
    {
        var receita = await _uof.ReceitaRepository
                                .Query()
                                .Where(r => r.Carteira != null && r.Carteira.UserId == _currentUser.UserId)
                                .FirstOrDefaultAsync(r => r.ReceitaId == id);

        if (receita is null)
        {
            _logger.LogWarning($"Não foi possível encontrar receita pelo ID {id}.");
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive);
        }

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receita));
    }

    public async Task<ApiResponse<PagedList<ReceitaDTO>>> ListarReceitasAsync(PaginationParams pagination)
    {
        var cached = await _cacheService.TryGetAsync<PagedList<ReceitaDTO>>(CacheKey());

        if (cached != null)
            return ApiResponse<PagedList<ReceitaDTO>>.Ok(cached);

        var query = _uof.ReceitaRepository
            .Query()
            .Where(r => r.Carteira != null && r.Carteira.UserId == _currentUser.UserId)
            .OrderByDescending(r => r.Data)
            .AsNoTracking()
            .ProjectToType<ReceitaDTO>();


        var receitasPaginadas = await PagedList<ReceitaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        await _cacheService.SetAsync(CacheKey(), receitasPaginadas, TimeSpan.FromMinutes(1));

        return ApiResponse<PagedList<ReceitaDTO>>.Ok(receitasPaginadas);
    }

    public async Task<ApiResponse<PagedList<ReceitaDTO>>> ReceitasFiltradasMaiorValorAsync(PaginationParams pagination)
    {
        var cached = await _cacheService.TryGetAsync<PagedList<ReceitaDTO>>(CacheKey());

        if (cached != null)
            return ApiResponse<PagedList<ReceitaDTO>>.Ok(cached);


        var query = _uof.ReceitaRepository
            .Query()
            .Where(r => r.Carteira != null && r.Carteira!.UserId == _currentUser.UserId)
            .OrderByDescending(r => r.Valor)
            .AsNoTracking()
            .ProjectToType<ReceitaDTO>();

        var receitasPaginadas = await PagedList<ReceitaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        await _cacheService.SetAsync(CacheKey(), receitasPaginadas, TimeSpan.FromMinutes(1));

        return ApiResponse<PagedList<ReceitaDTO>>.Ok(receitasPaginadas);
    }

    public async Task<ApiResponse<PagedList<ReceitaDTO>>> ReceitasFiltradasMenorValorAsync(PaginationParams pagination)
    {
        var cached = await _cacheService.TryGetAsync<PagedList<ReceitaDTO>>(CacheKey());

        if (cached != null)
            return ApiResponse<PagedList<ReceitaDTO>>.Ok(cached);

        var query = _uof.ReceitaRepository
            .Query()
            .Where(r => r.Carteira != null && r.Carteira!.UserId == _currentUser.UserId)
            .OrderBy(r => r.Valor)
            .AsNoTracking()
            .ProjectToType<ReceitaDTO>();

        var receitasPaginadas = await PagedList<ReceitaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        await _cacheService.SetAsync(CacheKey(), receitasPaginadas, TimeSpan.FromMinutes(1));

        return ApiResponse<PagedList<ReceitaDTO>>.Ok(receitasPaginadas);
    }

    public async Task<ApiResponse<ReceitaDTO>> CreateReceitaAsync(CreateReceitaDTO receitaDto)
    {
        var carteira = await _carteiraService.GetCarteiraAsync();

        if (carteira is null)
        {
            _logger.LogWarning($"Carteira do user é nula ou não existe, verificar se carteira já foi criada.");
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.WalletNotFound);
        }

        var receita = new Receita(receitaDto.Valor)
        {
            Titulo = receitaDto.Titulo,
            Descricao = receitaDto.Descricao,
            Data = receitaDto.Data,
            CarteiraId = carteira.CarteiraId,
        };

        carteira.Receitas.Add(receita);
        carteira.AdicionarSaldo(receitaDto.Valor);

        await _uof.ReceitaRepository.CreateAsync(receita);
        await _uof.CommitAsync();

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receita));
    }

    public async Task<ApiResponse<ReceitaDTO>> AtualizarReceitaAsync(Guid id, UpdateReceitaDTO receitaDto)
    {
        var receita = await _uof.ReceitaRepository
                                .Query()
                                .Where(r => r.Carteira!.UserId! == _currentUser.UserId)
                                .FirstOrDefaultAsync(r => r.ReceitaId == id);

        if (receita is null)
        {
            _logger.LogInformation($"Não foi possível atualizar receita de ID {id}, verifique os dados informados.");
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive);
        }

        receita.Titulo = receitaDto.Titulo;
        receita.Descricao = receitaDto.Descricao;
        receita.Data = receitaDto.Data;
        receita.Valor = receitaDto.Valor;
        receitaDto.UpdatedAt = DateTime.UtcNow;

        await _uof.ReceitaRepository.UpdateAsync(receita!);
        await _uof.CommitAsync();

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receita));
    }


    public async Task<ApiResponse<ReceitaDTO>> RemoverReceitaAsync(Guid id)
    {
        var carteira = await _carteiraService.GetCarteiraAsync();

        var receita = await _uof.ReceitaRepository
                                .Query()
                                .Where(r => r.CarteiraId == carteira.CarteiraId)
                                .FirstOrDefaultAsync(r => r.ReceitaId == id);

        if (receita is null)
        {
            _logger.LogWarning($"Não foi possível remover receita de ID {id}, não foi encontrada.");
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive);
        }

        if (carteira is null)
        {
            _logger.LogWarning($"Não foi encontrado carteira de ID {carteira?.CarteiraId}");
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.WalletNotFound);
        }

        carteira.DescontarSaldo(receita.Valor);
        carteira.Receitas.Remove(receita);

        await _uof.ReceitaRepository.DeleteAsync(receita);
        await _uof.CommitAsync();

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receita));
    }
}
