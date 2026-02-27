using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Enum;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface IMetaService
{
    Task<MetaDTO> CriarMetaAsync(CreateMetaDTO metaDto);
    Task<AporteMetasDTO> RegistrarAporteAsync(Guid metaId, CreateAporteMetaDTO aporteMetaDto);
    Task AtualizarMetaAsync(Guid metaId, UpdateMetaDTO metaDto);
    Task RemoverMetaAsync(Guid metaId);
    Task RemoverAporteAsync(Guid aporteMetaId);
    Task<MetaDTO> GetMetaPorIdAsync(Guid metaId);
    Task<AporteMetasDTO> GetAportePorIdAsync(Guid aporteMetaId);
    Task<IReadOnlyList<MetaDTO>> GetAllMetasAsync();
    Task<IReadOnlyList<AporteMetasDTO>> GetAllAportesDaMetaPorIdAsync(Guid metaId);
    Task FinalizarMeta(Guid metaId);
    Task<decimal> ValorTotalEmAportes(Guid metaId);


    // posteriormente será implementado filtros personalizados e pagination, colocar aqui para realizar.
}
