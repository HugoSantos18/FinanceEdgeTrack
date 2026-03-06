using FinanceEdgeTrack.Application.Dtos.Read.Lancamentos;
using FinanceEdgeTrack.Application.Dtos.Write.Lancamentos;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Error;
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

    public async Task<LancamentoDTO> LancarAsync(LancamentoDTO lancamentoDTO)
    {
        var lancamento = _mapper.Map<Lancamento>(lancamentoDTO);
        
        await _uof.LancamentoRepository.CreateAsync(lancamento);

        return _mapper.Map<LancamentoDTO>(lancamento);
    }

    public async Task AtualizarLancamentoAsync(Guid lancamentoId, UpdateLancamentoDTO lancamentoDto)
    {
        var lancamento = await _uof.LancamentoRepository.GetAsync(l => l.LancamentoId == lancamentoId);

        if (lancamento is null)
            throw new KeyNotFoundException(ResultMessages.NotFoundLancamento);

        lancamento.LancamentoId = lancamentoDto.LancamentoId;
            lancamento.DataLancamento = lancamentoDto.DataLancamento;
            lancamento.DespesaId = lancamentoDto.DespesaId;
            lancamento.ReceitaId = lancamentoDto.ReceitaId;
            lancamento.UserId = _currentUser.UserId;

        await _uof.LancamentoRepository.UpdateAsync(lancamento);
    }

    public async Task CancelarLancamentoAsync(Guid lancamentoId)
    {
        var lancamentoRemovido = await _uof.LancamentoRepository.GetAsync(l => l.LancamentoId == lancamentoId);

        if (lancamentoRemovido is null)
            throw new KeyNotFoundException(ResultMessages.NotFoundLancamento);

        await _uof.LancamentoRepository.DeleteAsync(lancamentoRemovido);
    }
    
    public async Task<LancamentoDTO> GetByIdAsync(Guid lancamentoId)
    {
        var lancamento = await _uof.LancamentoRepository.GetAsync(l => l.LancamentoId == lancamentoId);

        if (lancamento is null)
            throw new KeyNotFoundException(ResultMessages.NotFoundLancamento);

        return _mapper.Map<LancamentoDTO>(lancamento);
    }

    public async Task<IReadOnlyList<LancamentoDTO>> GetAllLancamentosAsync()
    {
        var lancamentos = await _uof.LancamentoRepository.GetAllAsync();

        return _mapper.Map<IReadOnlyList<LancamentoDTO>>(lancamentos);
    }

}
