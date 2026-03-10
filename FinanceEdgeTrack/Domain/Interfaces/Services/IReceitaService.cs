using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface IReceitaService
{
    Task<ApiResponse<ReceitaDTO>> CreateReceitaAsync(CreateReceitaDTO receitaDto);
    Task<ApiResponse<ReceitaDTO>> AtualizarReceitaAsync(Guid id, UpdateReceitaDTO receitaDto);
    Task<ApiResponse<ReceitaDTO>> RemoverReceitaAsync(Guid id);
    Task<ApiResponse<ReceitaDTO>> ObterReceitaPorIdAsync(Guid id);
    Task<ApiResponse<PagedList<ReceitaDTO>>> ListarReceitasAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<ReceitaDTO>>> ReceitasFiltradasMaiorValorAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<ReceitaDTO>>> ReceitasFiltradasMenorValorAsync(PaginationParams pagination);

}
