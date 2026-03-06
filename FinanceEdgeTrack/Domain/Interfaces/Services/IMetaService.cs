using FinanceEdgeTrack.Application.Common;
using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Enum;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface IMetaService
{
    Task<ApiResponse<MetaDTO>> CriarMetaAsync(CreateMetaDTO metaDto);
    Task<ApiResponse<AporteMetasDTO>> RegistrarAporteAsync(Guid metaId, CreateAporteMetaDTO aporteMetaDto);
    Task<ApiResponse<MetaDTO>> AtualizarMetaAsync(Guid metaId, UpdateMetaDTO metaDto);
    Task<ApiResponse<MetaDTO>> RemoverMetaAsync(Guid metaId);
    Task<ApiResponse<AporteMetasDTO>> RemoverAporteAsync(Guid aporteMetaId);
    Task<ApiResponse<MetaDTO>> GetMetaPorIdAsync(Guid metaId);
    Task<ApiResponse<AporteMetasDTO>> GetAportePorIdAsync(Guid aporteMetaId);
    Task<ApiResponse<IReadOnlyList<MetaDTO>>> GetAllMetasAsync();
    Task<ApiResponse<IReadOnlyList<AporteMetasDTO>>> GetAllAportesDaMetaPorIdAsync(Guid metaId);
    Task <ApiResponse<MetaDTO>> FinalizarMeta(Guid metaId);
    Task<ApiResponse<decimal>> ValorTotalEmAportes(Guid metaId);


    // posteriormente será implementado filtros personalizados e pagination, colocar aqui para realizar.
}
