using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;

namespace FinanceEdgeTrack.Domain.Interfaces.Services.Categories;

public interface IAporteMetasService
{
    Task<ApiResponse<AporteMetasDTO>> RegistrarAporteAsync(Guid metaId, CreateAporteMetaDTO dto);
    Task<ApiResponse<AporteMetasDTO>> RemoverAporteAsync(Guid aporteMetaId);
    Task<ApiResponse<AporteMetasDTO>> GetAporteByIdAsync(Guid aporteMetaId);
    Task<ApiResponse<PagedList<AporteMetasDTO>>> GetAportesDaMetaAsync(Guid metaId, PaginationParams pagination);
    Task<ApiResponse<decimal>> ValorTotalDaMetaAsync(Guid metaId);
}
