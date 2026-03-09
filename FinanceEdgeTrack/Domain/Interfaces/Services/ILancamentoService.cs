using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Lancamentos;
using FinanceEdgeTrack.Application.Dtos.Write.Lancamentos;
using FinanceEdgeTrack.Domain.Models;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface ILancamentoService
{
    Task<ApiResponse<LancamentoDTO>> LancarAsync(LancamentoDTO lancamentoDTO);
    Task<ApiResponse<LancamentoDTO>> DeletarLancamentoAsync(Guid lancamentoId);
    Task<ApiResponse<LancamentoDTO>> AtualizarLancamentoAsync(Guid lancamentoId, UpdateLancamentoDTO lancamentoDto);
    Task<ApiResponse<LancamentoDTO>> GetByIdAsync(Guid lancamentoId);
    Task<ApiResponse<IReadOnlyList<LancamentoDTO>>> GetAllLancamentosAsync();
}
