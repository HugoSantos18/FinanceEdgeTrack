using FinanceEdgeTrack.Application.Common;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface IReceitaService
{
    Task<ApiResponse<ReceitaDTO>> CreateReceitaAsync(CreateReceitaDTO receitaDto);
    Task<ApiResponse<ReceitaDTO>> AtualizarReceitaAsync(Guid id, UpdateReceitaDTO receitaDto);
    Task<ApiResponse<ReceitaDTO>> RemoverReceitaAsync(Guid id);
    Task<ApiResponse<ReceitaDTO>> ObterReceitaPorIdAsync(Guid id);
    Task<ApiResponse<IReadOnlyList<ReceitaDTO>>> ListarReceitasAsync();

    // métodos com filtros e paginação posteriormente.
}
