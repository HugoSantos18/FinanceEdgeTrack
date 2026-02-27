using FinanceEdgeTrack.Application.Dtos.Read.Lancamentos;
using FinanceEdgeTrack.Application.Dtos.Write.Lancamentos;
using FinanceEdgeTrack.Domain.Models;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface ILancamentoService
{
    Task<LancamentoDTO> LancarAsync(LancamentoDTO lancamentoDTO);
    Task CancelarLancamentoAsync(Guid lancamentoId);
    Task AtualizarLancamentoAsync(Guid lancamentoId, UpdateLancamentoDTO lancamentoDto);
    Task<LancamentoDTO> GetByIdAsync(Guid lancamentoId);
    Task<IReadOnlyList<LancamentoDTO>> GetAllLancamentosAsync();
}
