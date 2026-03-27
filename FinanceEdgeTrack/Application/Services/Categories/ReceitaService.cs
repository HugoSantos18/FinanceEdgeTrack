using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Application.Services.Auth;
using FinanceEdgeTrack.Domain.Interfaces;
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
    private readonly CurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly ILogger<ReceitaService> _logger;

    public ReceitaService(IUnitOfWork uof, IMapper mapper, ICarteiraService carteira, 
        CurrentUser currentUser, ILogger<ReceitaService> logger)
    {
        _mapper = mapper;
        _uof = uof;
        _carteiraService = carteira;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ApiResponse<ReceitaDTO>> ObterReceitaPorIdAsync(Guid id)
    {
        var receita = await _uof.ReceitaRepository.GetAsync(r => r.ReceitaId == id);

        if (receita is null)
        {
            _logger.LogInformation($"Não foi possível encontrar receita pelo ID {id}.");
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive);
        }

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receita));
    }

    public async Task<ApiResponse<PagedList<ReceitaDTO>>> ListarReceitasAsync(PaginationParams pagination)
    {
        var receitas = _uof.ReceitaRepository.GetAll();

        if (receitas is null)
        {
            _logger.LogInformation($"Não foi possível encontrar nenhuma receita, coleção possivelmente vazia.");
            return ApiResponse<PagedList<ReceitaDTO>>.Fail(ResultMessages.NotFoundReceive);
        }

        var query = receitas
            .AsNoTracking()
            .OrderByDescending(r => r.Data)
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
        var receitas = _uof.ReceitaRepository.GetAll();

        if (receitas is null)
        {
            _logger.LogInformation($"Não foi possível encontrar nenhuma receita, coleção possivelmente vazia.");
            return ApiResponse<PagedList<ReceitaDTO>>.Fail(ResultMessages.NotFoundReceive);
        }

        var query = receitas
            .AsNoTracking()
            .OrderByDescending(r => r.Valor)
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
        var receitas = _uof.ReceitaRepository.GetAll();

        if (receitas is null)
        {
            _logger.LogInformation($"Não foi possível encontrar nenhuma receita, coleção possivelmente vazia.");
            return ApiResponse<PagedList<ReceitaDTO>>.Fail(ResultMessages.NotFoundReceive);
        }

        var query = receitas
            .AsNoTracking()
            .OrderBy(r => r.Valor)
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
        var receita = _mapper.Map<Receita>(receitaDto);

        await _carteiraService.AdicionarSaldoAsync(receita.Valor);
        await _uof.ReceitaRepository.CreateAsync(receita);

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receita));
    }

    public async Task<ApiResponse<ReceitaDTO>> AtualizarReceitaAsync(Guid id, UpdateReceitaDTO receitaDto)
    {
        var receita = await _uof.ReceitaRepository.GetAsync(r => r.ReceitaId == id);

        if (receita is null)
        {
            _logger.LogInformation($"Não foi possível atualizar receita de ID {id}, verifique os dados informados.");
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive);
        }

        receita.Titulo = receitaDto.Titulo;
        receita.Descricao = receitaDto.Descricao;
        receita.Data = receitaDto.Data;
        receita.Valor = receitaDto.Valor;
        receitaDto.UpdatedAt = DateTime.UtcNow.ToShortDateString();

        await _uof.ReceitaRepository.UpdateAsync(receita!);

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receita));
    }


    public async Task<ApiResponse<ReceitaDTO>> RemoverReceitaAsync(Guid id)
    {
        var receitaRemovida = await _uof.ReceitaRepository.GetAsync(r => r.ReceitaId == id);

        if (receitaRemovida is null)
        {
            _logger.LogInformation($"Não foi possível remover receita de ID {id}, não encontrado.");
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive);
        }

        await _carteiraService.DescontarSaldoAsync(receitaRemovida.Valor);
        await _uof.ReceitaRepository.DeleteAsync(receitaRemovida!);

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receitaRemovida));
    }
}
