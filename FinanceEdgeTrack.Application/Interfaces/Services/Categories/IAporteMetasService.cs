using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Metas;
using FinanceEdgeTrack.Application.DTOs.Write.Categorias;

namespace FinanceEdgeTrack.Application.Interfaces.Services.Categories;

public interface IAporteMetasService
{
    Task<ApiResponse<AporteMetasDTO>> RegistrarAporteAsync(Guid metaId, CreateAporteMetaDTO dto);
    Task<ApiResponse<AporteMetasDTO>> RemoverAporteAsync(Guid aporteMetaId);
    Task<ApiResponse<AporteMetasDTO>> GetAporteByIdAsync(Guid aporteMetaId);
    Task<ApiResponse<PagedList<AporteMetasDTO>>> GetAportesDaMetaAsync(Guid metaId, PaginationParams pagination);
    Task<ApiResponse<decimal>> ValorTotalDaMetaAsync(Guid metaId);
}
