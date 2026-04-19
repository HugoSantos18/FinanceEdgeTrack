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

namespace FinanceEdgeTrack.Application.Services.Categories
{
    public class DespesaService : IDespesaService
    {
        private readonly IUnitOfWork _uof;
        private readonly ICarteiraService _carteiraService;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<DespesaService> _logger;
        private readonly ICacheService _cacheService;

        public DespesaService(IUnitOfWork uof, IMapper mapper, ICarteiraService carteira,
            ICurrentUser currentUser, ILogger<DespesaService> logger, ICacheService cache)
        {
            _mapper = mapper;
            _uof = uof;
            _carteiraService = carteira;
            _currentUser = currentUser;
            _logger = logger;
            _cacheService = cache;
        }
        private string CacheKey()
        {
            return _cacheService.SetCacheKey(_currentUser.UserId);
        }

        public async Task<ApiResponse<DespesaDTO>> ObterDespesaPorIdAsync(Guid id)
        {
            var despesa = await _uof.DespesaRepository
                                    .Query()
                                    .Where(d => d.Carteira!.UserId! == _currentUser.UserId)
                                    .FirstOrDefaultAsync(d => d.DespesaId == id);

            if (despesa is null)
            {
                _logger.LogInformation($"Não foi possível encontrar a despesa pelo ID {id}");
                return ApiResponse<DespesaDTO>.Fail(ResultMessages.NotFoundDespesa);
            }

            var despesaDto = _mapper.Map<DespesaDTO>(despesa);

            return ApiResponse<DespesaDTO>.Ok(despesaDto);
        }

        public async Task<ApiResponse<PagedList<DespesaDTO>>> ListarDespesasAsync(PaginationParams pagination)
        {
            var query = _uof.DespesaRepository
                .Query()
                .Where(d => d.Carteira != null && d.Carteira!.UserId == _currentUser.UserId)
                .OrderByDescending(d => d.Data)
                .AsNoTracking()
                .ProjectToType<DespesaDTO>();


            var despesasPaginadas = await PagedList<DespesaDTO>.CreateAsync
                (
                query,
                pagination.PageNumber,
                pagination.PageSize
                );

            return ApiResponse<PagedList<DespesaDTO>>.Ok(despesasPaginadas);
        }

        public async Task<ApiResponse<PagedList<DespesaDTO>>> DespesasFixasPaginadasAsync(PaginationParams pagination)
        {
            var cached = await _cacheService.TryGetAsync<PagedList<DespesaDTO>>(CacheKey());

            if (cached != null)
                return ApiResponse<PagedList<DespesaDTO>>.Ok(cached);

            var query = _uof.DespesaRepository
                .Query()
                .Where(d => d.Carteira != null && d.Carteira!.UserId == _currentUser.UserId)
                .Where(d => d.Fixa == true)
                .OrderByDescending(d => d.Valor)
                .AsNoTracking()
                .ProjectToType<DespesaDTO>();

            var despesasPaginadas = await PagedList<DespesaDTO>.CreateAsync
                (
                query,
                pagination.PageNumber,
                pagination.PageSize
                );

            await _cacheService.SetAsync(CacheKey(), despesasPaginadas, TimeSpan.FromMinutes(1));

            return ApiResponse<PagedList<DespesaDTO>>.Ok(despesasPaginadas);
        }

        public async Task<ApiResponse<PagedList<DespesaDTO>>> DespesasFiltradasMaiorValorAsync(PaginationParams pagination)
        {
            var cached = await _cacheService.TryGetAsync<PagedList<DespesaDTO>>(CacheKey());

            if (cached != null)
                return ApiResponse<PagedList<DespesaDTO>>.Ok(cached);

            var query = _uof.DespesaRepository
                .Query()
                .Where(d => d.Carteira != null && d.Carteira!.UserId == _currentUser.UserId)
                .OrderByDescending(d => d.Valor)
                .AsNoTracking()
                .ProjectToType<DespesaDTO>();

            var despesasFiltradas = await PagedList<DespesaDTO>.CreateAsync
                (
                query,
                pagination.PageNumber,
                pagination.PageSize
                );

            await _cacheService.SetAsync(CacheKey(), despesasFiltradas, TimeSpan.FromMinutes(1));

            return ApiResponse<PagedList<DespesaDTO>>.Ok(despesasFiltradas);
        }

        public async Task<ApiResponse<PagedList<DespesaDTO>>> DespesasFiltradasMenorValorAsync(PaginationParams pagination)
        {
            var cached = await _cacheService.TryGetAsync<PagedList<DespesaDTO>>(CacheKey());

            if (cached != null)
                return ApiResponse<PagedList<DespesaDTO>>.Ok(cached);

            var query = _uof.DespesaRepository
            .Query()
            .Where(d => d.Carteira != null && d.Carteira!.UserId == _currentUser.UserId)
            .OrderBy(d => d.Valor)
            .AsNoTracking()
            .ProjectToType<DespesaDTO>();

            var despesasFiltradas = await PagedList<DespesaDTO>.CreateAsync
                (
                query,
                pagination.PageNumber,
                pagination.PageSize
                );

            await _cacheService.SetAsync(CacheKey(), despesasFiltradas, TimeSpan.FromMinutes(1));

            return ApiResponse<PagedList<DespesaDTO>>.Ok(despesasFiltradas);
        }

        public async Task<ApiResponse<DespesaDTO>> CreateDespesaAsync(CreateDespesaDTO despesaDto)
        {
            var carteira = await _carteiraService.GetCarteiraAsync();

            if (carteira is null)
            {
                _logger.LogWarning($"Não foi encontrado a carteira de ID {carteira?.CarteiraId}");
                return ApiResponse<DespesaDTO>.Fail(ResultMessages.WalletNotFound);
            }

            var despesa = new Despesa(despesaDto.Valor, despesaDto.Fixa)
            {
                Titulo = despesaDto.Titulo,
                Descricao = despesaDto.Descricao,
                Data = despesaDto.Data,
                CarteiraId = carteira.CarteiraId
            };

            carteira.Despesas.Add(despesa);
            carteira.DescontarSaldo(despesa.Valor);

            await _uof.DespesaRepository.CreateAsync(despesa);
            await _uof.CommitAsync();

            return ApiResponse<DespesaDTO>.Ok(_mapper.Map<DespesaDTO>(despesa));
        }

        public async Task<ApiResponse<DespesaDTO>> AtualizarDespesaAsync(Guid id, UpdateDespesaDTO despesaDto)
        {
            var despesa = await _uof.DespesaRepository
                                    .Query()
                                    .Where(d => d.Carteira!.UserId! == _currentUser.UserId)
                                    .FirstOrDefaultAsync(d => d.DespesaId == id);

            if (despesa is null)
            {
                _logger.LogInformation($"Não foi possível atualizar despesa de ID: {id}, verifique as credenciais ou o ID informado.");
                return ApiResponse<DespesaDTO>.Fail(ResultMessages.NotFoundDespesa);
            }

            despesa.Titulo = despesaDto.Titulo!;
            despesa.Descricao = despesaDto.Descricao;
            despesa.Data = despesaDto.Data;
            despesa.Valor = despesaDto.Valor;
            despesaDto.UpdatedAt = DateTime.UtcNow;

            await _uof.DespesaRepository.UpdateAsync(despesa);
            await _uof.CommitAsync();

            return ApiResponse<DespesaDTO>.Ok(_mapper.Map<DespesaDTO>(despesa));
        }


        public async Task<ApiResponse<DespesaDTO>> RemoverDespesaAsync(Guid id)
        {
            var despesa = await _uof.DespesaRepository
                                   .Query()
                                   .Where(d => d.Carteira!.UserId! == _currentUser.UserId)
                                   .FirstOrDefaultAsync(d => d.DespesaId == id);

            if (despesa is null)
            {
                _logger.LogInformation($"Não foi possível remover despesa de ID: {id}, verifique o ID informado.");
                return ApiResponse<DespesaDTO>.Fail(ResultMessages.NotFoundDespesa);
            }

            await _carteiraService.AdicionarSaldoAsync(despesa.Valor);
            await _uof.DespesaRepository.DeleteAsync(despesa);

            await _uof.CommitAsync();

            return ApiResponse<DespesaDTO>.Ok(_mapper.Map<DespesaDTO>(despesa), "Despesa removida com sucesso");
        }
    }
}
