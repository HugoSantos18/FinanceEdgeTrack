using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Categorias;
using FinanceEdgeTrack.Application.DTOs.Write.Categorias;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Application.Interfaces.Services.Auth;
using FinanceEdgeTrack.Application.Interfaces.Services.Carteira;
using FinanceEdgeTrack.Application.Interfaces.Services.Categories;
using FinanceEdgeTrack.Domain.Entities;
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

        public DespesaService(IUnitOfWork uof, IMapper mapper, ICarteiraService carteira,
            ICurrentUser currentUser, ILogger<DespesaService> logger)
        {
            _mapper = mapper;
            _uof = uof;
            _carteiraService = carteira;
            _currentUser = currentUser;
            _logger = logger;
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
            var query = _uof.DespesaRepository
                .Query()
                .Where(d => d.Carteira != null && d.Carteira!.UserId == _currentUser.UserId)
                .Where(d => d.Fixa == true)
                .OrderByDescending(d => d.Valor)
                .AsNoTracking()
                .ProjectToType<DespesaDTO>();

            var despesasFiltradas = await PagedList<DespesaDTO>.CreateAsync
                (
                query,
                pagination.PageNumber,
                pagination.PageSize
                );

            return ApiResponse<PagedList<DespesaDTO>>.Ok(despesasFiltradas);
        }

        public async Task<ApiResponse<PagedList<DespesaDTO>>> DespesasFiltradasMaiorValorAsync(PaginationParams pagination)
        {
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

            return ApiResponse<PagedList<DespesaDTO>>.Ok(despesasFiltradas);
        }

        public async Task<ApiResponse<PagedList<DespesaDTO>>> DespesasFiltradasMenorValorAsync(PaginationParams pagination)
        {
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

            return ApiResponse<PagedList<DespesaDTO>>.Ok(despesasFiltradas);
        }

        public async Task<ApiResponse<DespesaDTO>> CreateDespesaAsync(CreateDespesaDTO despesaDto)
        {
            var carteira = await _carteiraService.GetCarteiraAsync();

            if (carteira is null)
            {
                _logger.LogWarning("Não foi encontrada a carteira para o usuário corrente.");
                return ApiResponse<DespesaDTO>.Fail(ResultMessages.WalletNotFound);
            }

            await using var tx = await _uof.BeginTransactionAsync();

            var debitou = await _carteiraService.DebitarSaldoComGuardaAsync(carteira.CarteiraId, despesaDto.Valor);
            if (!debitou)
            {
                await tx.RollbackAsync();
                return ApiResponse<DespesaDTO>.Fail(ResultMessages.InsufficientBalance);
            }

            var despesa = new Despesa(despesaDto.Valor, despesaDto.Fixa)
            {
                Titulo = despesaDto.Titulo,
                Descricao = despesaDto.Descricao,
                Data = despesaDto.Data,
                CarteiraId = carteira.CarteiraId
            };

            await _uof.DespesaRepository.CreateAsync(despesa);
            await _uof.CommitAsync();
            await tx.CommitAsync();

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

            await using var tx = await _uof.BeginTransactionAsync();

            if (despesaDto.Valor > despesa.Valor)
                await _carteiraService.CreditarSaldoAsync(despesa.CarteiraId, despesaDto.Valor - despesa.Valor); // Reembolsa a diferença para o usuário

            else if (despesaDto.Valor < despesa.Valor)
                await _carteiraService.DebitarSaldoComGuardaAsync(despesa.CarteiraId, despesaDto.Valor - despesa.Valor); // Debita a diferença da carteira do usuário

            else { await tx.RollbackAsync(); return ApiResponse<DespesaDTO>.Fail(ResultMessages.InvalidPrice); } // Caso o valor seja inválido, retorna erro de saldo inválido

            despesa.Titulo = despesaDto.Titulo!;
            despesa.Descricao = despesaDto.Descricao;
            despesa.Data = despesaDto.Data;
            despesa.Valor = despesaDto.Valor;
            despesaDto.UpdatedAt = DateTime.UtcNow;

            await _uof.DespesaRepository.UpdateAsync(despesa);
            await _uof.CommitAsync();
            await tx.CommitAsync();

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

            await using var tx = await _uof.BeginTransactionAsync();

            await _uof.DespesaRepository.DeleteAsync(despesa);
            await _uof.CommitAsync();
            await _carteiraService.CreditarSaldoAsync(despesa.CarteiraId, despesa.Valor);
            await tx.CommitAsync();

            return ApiResponse<DespesaDTO>.Ok(_mapper.Map<DespesaDTO>(despesa), "Despesa removida com sucesso");
        }
    }
}
