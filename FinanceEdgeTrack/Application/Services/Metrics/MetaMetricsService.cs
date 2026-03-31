using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Metas;
using FinanceEdgeTrack.Application.Services.Auth;
using FinanceEdgeTrack.Domain.Enum;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Metrics;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Application.Services.Metrics;

public class MetaMetricsService : IMetaMetrics
{
    private readonly CurrentUser _currentUser;
    private readonly ILogger<MetaMetricsService> _logger;
    private readonly IUnitOfWork _uof;

    public MetaMetricsService(CurrentUser currentUser, ILogger<MetaMetricsService> logger, IUnitOfWork uof)
    {
        _currentUser = currentUser;
        _logger = logger;
        _uof = uof;
    }

    public async Task<ApiResponse<MetasKPIsDTO>> GetKPIsMetas(int month)
    {
        var query = _uof.MetaRepository
                    .Query()
                    .AsNoTracking();

        var metas = await query.ToListAsync();

        if (metas is null)
            return ApiResponse<MetasKPIsDTO>.Fail($"{ResultMessages.EmptyMetaCollection}");

        int metasIniciadasNoMes = metas
                                 .Where(m => m.DataInicio.Month == month)
                                 .Count();


        int metasConcluidasNoMes = metas
                                  .Where(m => m.DataConclusao!.Value.Month == month)
                                  .Where(m => m.Status == Status.Concluido)
                                  .Where(m => m.DataConclusao != null)
                                  .Count();


        int metasPendentesNoMes = metas
                                 .Where(m => m.Status == Status.EmAberto)
                                 .Count();

        int metasCanceladasNoMes = metas
                                  .Where(m => m.Status == Status.Cancelada)
                                  .Count();

        decimal valorRestanteParaCompletarTodas = metas
                                                 .Where(m => m.Status == Status.EmAberto)
                                                 .Sum(m => m.ValorRestanteParaCompletar());

        double mediaDias = metas
                           .Where(m => m.Status == Status.Concluido)
                           .Where(m => m.DataConclusao != null)
                           .Average(m => (m.DataConclusao!.Value - m.DataInicio).TotalDays);


        var metasKpisDTO = new MetasKPIsDTO
        {
            MetasIniciadasNoMes = metasIniciadasNoMes,
            TotalConcluidasNoMes = metasConcluidasNoMes,
            TotalPendentesNoMes = metasPendentesNoMes,
            TotalCanceladasNoMes = metasCanceladasNoMes,
            ValorTotalRestanteParaCompletar = valorRestanteParaCompletarTodas,
            MediaDiasParaCompletar = mediaDias
        };


        return ApiResponse<MetasKPIsDTO>.Ok( metasKpisDTO, "KPIs");
    }

    public async Task<ApiResponse<MetasResumoGeralDTO>> GetMetricsMetas()
    {
        var query = _uof.MetaRepository
                        .Query()
                        .AsNoTracking();

        var metas = await query.ToListAsync();

        if (metas is null)
            return ApiResponse<MetasResumoGeralDTO>.Fail($"{ResultMessages.EmptyMetaCollection}");

        decimal totalValorAlvo = metas
                                 .Sum(m => m.ValorAlvo);

        decimal totalAportado = metas
                                .Sum(m => m.ValorAtual);

        int totalConcluidas = metas
                              .Where(m => m.Status == Status.Concluido)
                              .Count();

        decimal percentualConcluido = ((totalConcluidas / metas.Count()) * 100);

        if (percentualConcluido > 0 && totalValorAlvo != 0)
            return ApiResponse<MetasResumoGeralDTO>.Fail($"{ResultMessages.InsuficientData}");


        var metasResumoDTO = new MetasResumoGeralDTO
        {
            TotalValorAlvo = totalValorAlvo,
            TotalAportado = totalAportado,
            PercentualConclusao = percentualConcluido
        };

        return ApiResponse<MetasResumoGeralDTO>.Ok(metasResumoDTO, $"Metas gerais");
    }

    public async Task<ApiResponse<MetasResumoMensalDTO>> GetMetricsMetasNoMes(int month)
    {
        var query = _uof.MetaRepository
                      .Query()
                      .Where(m => m.DataInicio.Month == month)
                      .AsNoTracking();

        var metas = await query.ToListAsync();

        if (metas is null)
            return ApiResponse<MetasResumoMensalDTO>.Fail($"{ResultMessages.EmptyMetaCollection}");

        decimal totalValorAlvoNoMes = metas
                                      .Sum(m => m.ValorAlvo);

        decimal totalAportadoNoMes = metas
                                     .Sum(m => m.ValorAtual);


        var metasResumoMensalDTO = new MetasResumoMensalDTO
        {
            TotalValorAlvoNoMes = totalValorAlvoNoMes,
            TotalAportadoNoMes = totalAportadoNoMes,
        };

        return ApiResponse<MetasResumoMensalDTO>.Ok(metasResumoMensalDTO, $"Metas mensais");
    }
}
