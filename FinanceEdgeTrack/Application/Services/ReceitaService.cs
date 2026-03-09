using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using MapsterMapper;

namespace FinanceEdgeTrack.Application.Services;

public class ReceitaService : IReceitaService
{

    private readonly IUnitOfWork _uof;
    private readonly ICarteiraService _carteiraService;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public ReceitaService(IUnitOfWork uof, IMapper mapper, ICarteiraService carteira, ICurrentUserService currentUser)
    {
        this._mapper = mapper;
        this._uof = uof;
        this._carteiraService = carteira;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<ReceitaDTO>> ObterReceitaPorIdAsync(Guid id)
    {
        var receita = await _uof.ReceitaRepository.GetAsync(r => r.ReceitaId == id);

        if (receita is null)
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive);

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receita));
    }

    public async Task<ApiResponse<IReadOnlyList<ReceitaDTO>>> ListarReceitasAsync()
    {
        var receitas = await _uof.ReceitaRepository.GetAllAsync();

        if (receitas is null)
            return ApiResponse<IReadOnlyList<ReceitaDTO>>.Fail(ResultMessages.NotFoundReceive);

        return ApiResponse<IReadOnlyList<ReceitaDTO>>.Ok(_mapper.Map<IReadOnlyList<ReceitaDTO>>(receitas));
    }

    public async Task<ApiResponse<ReceitaDTO>> CreateReceitaAsync(CreateReceitaDTO receitaDto)
    {
        var receita = _mapper.Map<Receita>(receitaDto);

        await _carteiraService.AdicionarSaldoAsync(_currentUser.UserId, receita.Valor);
        await _uof.ReceitaRepository.CreateAsync(receita);

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receita));
    }

    public async Task<ApiResponse<ReceitaDTO>> AtualizarReceitaAsync(Guid id, UpdateReceitaDTO receitaDto)
    {
        var receita = await _uof.ReceitaRepository.GetAsync(r => r.ReceitaId == id);

        if (receita is null)
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive);

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
            return ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive);

        await _carteiraService.DescontarSaldoAsync(_currentUser.UserId, receitaRemovida.Valor);
        await _uof.ReceitaRepository.DeleteAsync(receitaRemovida!);

        return ApiResponse<ReceitaDTO>.Ok(_mapper.Map<ReceitaDTO>(receitaRemovida));
    }
}
