using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Lancamentos;
using FinanceEdgeTrack.Application.Dtos.Write.Lancamentos;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using MapsterMapper;

namespace FinanceEdgeTrack.Application.Services;

public class LancamentoService : ILancamentoService
{
    private readonly IUnitOfWork _uof;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public LancamentoService(IUnitOfWork uof, ICurrentUserService currentUser, IMapper mapper)
    {
        _uof = uof;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<LancamentoDTO>> LancarAsync(LancamentoDTO lancamentoDTO)
    {
        var lancamento = _mapper.Map<Lancamento>(lancamentoDTO);

        if (lancamento is null)
            return ApiResponse<LancamentoDTO>.Fail(ResultMessages.ErrorCreation);

        await _uof.LancamentoRepository.CreateAsync(lancamento);

        return ApiResponse<LancamentoDTO>.Ok(_mapper.Map<LancamentoDTO>(lancamento));
    }

    public async Task<ApiResponse<LancamentoDTO>> AtualizarLancamentoAsync(Guid lancamentoId, UpdateLancamentoDTO lancamentoDto)
    {
        var lancamento = await _uof.LancamentoRepository.GetAsync(l => l.LancamentoId == lancamentoId);

        if (lancamento is null)
            return ApiResponse<LancamentoDTO>.Fail(ResultMessages.ErrorUpdate);

        lancamento.LancamentoId = lancamentoDto.LancamentoId;
        lancamento.DataLancamento = lancamentoDto.DataLancamento;
        lancamento.DespesaId = lancamentoDto.DespesaId;
        lancamento.ReceitaId = lancamentoDto.ReceitaId;
        lancamento.UserId = _currentUser.UserId;
        lancamentoDto.UpdatedAt = DateTime.UtcNow.ToShortDateString();

        await _uof.LancamentoRepository.UpdateAsync(lancamento);

        return ApiResponse<LancamentoDTO>.Ok(_mapper.Map<LancamentoDTO>(lancamento));
    }

    public async Task<ApiResponse<LancamentoDTO>> DeletarLancamentoAsync(Guid lancamentoId)
    {
        var lancamentoRemovido = await _uof.LancamentoRepository.GetAsync(l => l.LancamentoId == lancamentoId);

        if (lancamentoRemovido is null)
            return ApiResponse<LancamentoDTO>.Fail(ResultMessages.NotFoundLancamento);

        await _uof.LancamentoRepository.DeleteAsync(lancamentoRemovido);

        return ApiResponse<LancamentoDTO>.Ok(_mapper.Map<LancamentoDTO>(lancamentoRemovido));
    }

    public async Task<ApiResponse<LancamentoDTO>> GetByIdAsync(Guid lancamentoId)
    {
        var lancamento = await _uof.LancamentoRepository.GetAsync(l => l.LancamentoId == lancamentoId);

        if (lancamento is null)
            return ApiResponse<LancamentoDTO>.Fail(ResultMessages.NotFoundLancamento);

        return ApiResponse<LancamentoDTO>.Ok(_mapper.Map<LancamentoDTO>(lancamento));
    }

    public async Task<ApiResponse<IReadOnlyList<LancamentoDTO>>> GetAllLancamentosAsync()
    {
        var lancamentos = await _uof.LancamentoRepository.GetAllAsync();

        if (lancamentos is null)
            return ApiResponse<IReadOnlyList<LancamentoDTO>>.Fail(ResultMessages.NotFoundLancamento);

        return ApiResponse<IReadOnlyList<LancamentoDTO>>.Ok(_mapper.Map<IReadOnlyList<LancamentoDTO>>(lancamentos));
    }

}
