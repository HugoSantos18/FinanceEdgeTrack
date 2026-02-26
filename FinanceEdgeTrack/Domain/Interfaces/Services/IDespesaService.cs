using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface IDespesaService
{
    Task<DespesaDTO> CreateDespesaAsync(DespesaDTO despesaDto);
    
    Task AtualizarDespesaAsync(Guid id, UpdateDespesaDTO despesaDto);
 
    Task RemoverDespesaAsync(Guid id);

    Task<DespesaDTO> ObterDespesaPorIdAsync(Guid id);

    Task<IReadOnlyList<DespesaDTO>> ListarDespesasAsync();
}
