using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Metas;
using FinanceEdgeTrack.Application.DTOs.Write.Categorias;

namespace FinanceEdgeTrack.Application.Interfaces.Services.Categories;

public interface IMetaService
{
    Task<ApiResponse<MetaDTO>> CriarMetaAsync(CreateMetaDTO metaDto);
    Task<ApiResponse<MetaDTO>> AtualizarMetaAsync(Guid metaId, UpdateMetaDTO metaDto);
    Task<ApiResponse<MetaDTO>> RemoverMetaAsync(Guid metaId);
    Task<ApiResponse<MetaDTO>> GetMetaPorIdAsync(Guid metaId);
    Task<ApiResponse<PagedList<MetaDTO>>> GetAllMetasAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaiorValorAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMenorValorAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasQuaseConcluidasAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaisAntigaAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaisRecentesAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasPorStatusAsync(StatusParams statusPagination);
}
