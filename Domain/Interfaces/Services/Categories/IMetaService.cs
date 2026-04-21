using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Pagination.Filters;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;

namespace FinanceEdgeTrack.Domain.Interfaces.Services.Categories;

public interface IMetaService
{
    Task<ApiResponse<MetaDTO>> CriarMetaAsync(CreateMetaDTO metaDto);
    Task<ApiResponse<MetaDTO>> RegistrarAporteAsync(Guid metaId, CreateAporteMetaDTO aporteMetaDto);
    Task<ApiResponse<MetaDTO>> AtualizarMetaAsync(Guid metaId, UpdateMetaDTO metaDto);
    Task<ApiResponse<MetaDTO>> RemoverMetaAsync(Guid metaId);
    Task<ApiResponse<MetaDTO>> RemoverAporteAsync(Guid aporteMetaId);
    Task<ApiResponse<MetaDTO>> GetMetaPorIdAsync(Guid metaId);
    Task<ApiResponse<AporteMetasDTO>> GetAportePorIdAsync(Guid aporteMetaId);
    Task<ApiResponse<PagedList<MetaDTO>>> GetAllMetasAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<AporteMetasDTO>>> GetAllAportesDaMetaPorIdAsync(Guid metaId, PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaiorValorAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMenorValorAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasQuaseConcluidasAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaisAntigaAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaisRecentesAsync(PaginationParams pagination);
    Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasPorStatusAsync(StatusParams statusPagination);
    Task<ApiResponse<decimal>> ValorTotalEmAportes(Guid metaId);
}
