using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Application.Dtos.Read.Auth;
using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Domain.Models;
using Mapster;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;


namespace FinanceEdgeTrack.Application.Mappings;

public static class MapsterMappingConfig
{
    /// <summary>
    /// Método que configura e faz o mapeamento das Entidades --> Dto
    /// </summary>
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


        TypeAdapterConfig<ApplicationUser, ApplicationUserDTO>
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
