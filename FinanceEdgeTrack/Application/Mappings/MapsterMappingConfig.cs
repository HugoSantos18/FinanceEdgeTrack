using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Domain.Models.Abstract;
using FinanceEdgeTrack.Application.Dtos.Read.Lancamentos;
using FinanceEdgeTrack.Application.Dtos.Read.Auth;
using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Domain.Models;
using Mapster;


namespace FinanceEdgeTrack.Application.Mappings;

public static class MapsterMappingConfig
{
    /// <summary>
    /// Método que configura e faz o mapeamento das Entidades --> Dto
    /// </summary>
    public static void ConfigurarMapeamento()
    {
        TypeAdapterConfig<Categoria, CategoriaBaseDTO>
            .NewConfig();

        TypeAdapterConfig<ApplicationUser, ApplicationUserDTO>
            .NewConfig();

        TypeAdapterConfig<Lancamento, LancamentoDTO>
            .NewConfig();

        TypeAdapterConfig<AporteMetas, AporteMetasDTO>
            .NewConfig();

        TypeAdapterConfig<Carteira, CarteiraDTO>
            .NewConfig();

        TypeAdapterConfig<Despesa, DespesaDTO>
            .NewConfig();

        TypeAdapterConfig<Receita, ReceitaDTO>
            .NewConfig();

        TypeAdapterConfig<Meta, MetaDTO>
            .NewConfig();
    }
}
