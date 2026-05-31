using FinanceEdgeTrack.Application.DTOs.Read.Categorias;
using FinanceEdgeTrack.Application.DTOs.Read.Metas;
using FinanceEdgeTrack.Application.DTOs.Write.Categorias;
using FinanceEdgeTrack.Domain.Entities;
using Mapster;

namespace FinanceEdgeTrack.Application.Mappings;

public static class MapsterMappingConfig
{
    public static void ConfigurarMapeamento()
    {
        TypeAdapterConfig<CreateDespesaDTO, Despesa>
            .NewConfig();

        TypeAdapterConfig<CreateMetaDTO, Meta>
            .NewConfig();

        TypeAdapterConfig<CreateReceitaDTO, Receita>
            .NewConfig();

        TypeAdapterConfig<CreateAporteMetaDTO, AporteMetas>
            .NewConfig();

        TypeAdapterConfig<UpdateDespesaDTO, Despesa>
            .NewConfig();

        TypeAdapterConfig<UpdateMetaDTO, Meta>
            .NewConfig();

        TypeAdapterConfig<UpdateReceitaDTO, Receita>
            .NewConfig();

        TypeAdapterConfig<AporteMetas, AporteMetasDTO>
            .NewConfig();

        TypeAdapterConfig<Despesa, DespesaDTO>
            .NewConfig();

        TypeAdapterConfig<Receita, ReceitaDTO>
            .NewConfig();

        TypeAdapterConfig<Meta, MetaDTO>
            .NewConfig();
    }
}
