using FinanceEdgeTrack.Application.Common;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface IDespesaService
{
    Task<ApiResponse<DespesaDTO>> CreateDespesaAsync(CreateDespesaDTO despesaDto);
    
    Task<ApiResponse<DespesaDTO>> AtualizarDespesaAsync(Guid id, UpdateDespesaDTO despesaDto);
 
    Task<ApiResponse<DespesaDTO>> RemoverDespesaAsync(Guid id);

    Task<ApiResponse<DespesaDTO>> ObterDespesaPorIdAsync(Guid id);

    Task<ApiResponse<IReadOnlyList<DespesaDTO>>> ListarDespesasAsync();
}
