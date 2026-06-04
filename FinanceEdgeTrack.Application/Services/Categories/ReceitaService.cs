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

namespace FinanceEdgeTrack.Application.Services.Categories;

public class ReceitaService : IReceitaService
{

    private readonly IUnitOfWork _uof;
    private readonly ICarteiraService _carteiraService;
    private readonly IMapper _mapper;
    private readonly ILogger<ReceitaService> _logger;
    private readonly ICurrentUser _currentUser;

    public ReceitaService(IUnitOfWork uof, IMapper mapper, ICarteiraService carteira,
                          ILogger<ReceitaService> logger, ICurrentUser currentUser)
    {
        _mapper = mapper;
        _uof = uof;
        _carteiraService = carteira;
        _logger = logger;
        _currentUser = currentUser;
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

        return ApiResponse<PagedList<ReceitaDTO>>.Ok(receitasPaginadas);
    }

    public async Task<ApiResponse<PagedList<ReceitaDTO>>> ReceitasFiltradasMaiorValorAsync(PaginationParams pagination)
    {
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

        return ApiResponse<PagedList<ReceitaDTO>>.Ok(receitasPaginadas);
    }

    public async Task<ApiResponse<PagedList<ReceitaDTO>>> ReceitasFiltradasMenorValorAsync(PaginationParams pagination)
    {
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

        return ApiResponse<PagedList<ReceitaDTO>>.Ok(receitasPaginadas);
    }

    public async Task<ApiResponse<ReceitaDTO>> CreateReceitaAsync(CreateReceitaDTO receitaDto)
    {
        var carteira = await _carteiraService.GetCarteiraAsync();

        if (carteira is null)
        {
            _logger.LogWarning("Carteira do user é nula ou não existe, verificar se carteira já foi criada.");
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.WalletNotFound);
        }

        var receita = new Receita(receitaDto.Valor)
        {
            Titulo = receitaDto.Titulo,
            Descricao = receitaDto.Descricao,
            Data = receitaDto.Data,
            CarteiraId = carteira.CarteiraId,
        };

        await using var tx = await _uof.BeginTransactionAsync();

        await _uof.ReceitaRepository.CreateAsync(receita);
        await _uof.CommitAsync();
        await _carteiraService.CreditarSaldoAsync(carteira.CarteiraId, receitaDto.Valor);
        await tx.CommitAsync();

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

        await using var tx = await _uof.BeginTransactionAsync();

        if (receita.Valor > receita.Valor)
            await _carteiraService.CreditarSaldoAsync(receita.CarteiraId, receitaDto.Valor - receita.Valor); // Reembolsa a diferença para o usuário

        else if (receitaDto.Valor < receita.Valor)
            await _carteiraService.DebitarSaldoComGuardaAsync(receita.CarteiraId, receitaDto.Valor - receita.Valor); // Debita a diferença da carteira do usuário

        else if (receitaDto.Valor == receita.Valor) { _logger.LogInformation("Saldo não alterado"); } // Não altera o saldo da carteira do usuário

        else { await tx.RollbackAsync(); return ApiResponse<ReceitaDTO>.Fail(ResultMessages.InvalidPrice); } // Caso o valor seja inválido, retorna erro de saldo inválido

        receita.Titulo = receitaDto.Titulo;
        receita.Descricao = receitaDto.Descricao;
        receita.Data = receitaDto.Data;
        receita.Valor = receitaDto.Valor;
        receitaDto.UpdatedAt = DateTime.UtcNow;

        await _uof.ReceitaRepository.UpdateAsync(receita!);
        await _uof.CommitAsync();
        await tx.CommitAsync();

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receita));
    }


    public async Task<ApiResponse<ReceitaDTO>> RemoverReceitaAsync(Guid id)
    {
        var carteira = await _carteiraService.GetCarteiraAsync();

        if (carteira is null)
        {
            _logger.LogWarning("Carteira do usuário corrente não foi encontrada.");
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.WalletNotFound);
        }

        var receita = await _uof.ReceitaRepository
                                .Query()
                                .Where(r => r.CarteiraId == carteira.CarteiraId)
                                .FirstOrDefaultAsync(r => r.ReceitaId == id);

        if (receita is null)
        {
            _logger.LogWarning($"Não foi possível remover receita de ID {id}, não foi encontrada.");
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive);
        }

        await using var tx = await _uof.BeginTransactionAsync();

        var debitou = await _carteiraService.DebitarSaldoComGuardaAsync(carteira.CarteiraId, receita.Valor);
        if (!debitou)
        {
            await tx.RollbackAsync();
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.InsufficientBalanceToRevert);
        }

        await _uof.ReceitaRepository.DeleteAsync(receita);
        await _uof.CommitAsync();
        await tx.CommitAsync();

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receita));
    }
}
