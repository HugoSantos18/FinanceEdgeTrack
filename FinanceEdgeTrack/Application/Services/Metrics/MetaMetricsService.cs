using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Metas;
using FinanceEdgeTrack.Domain.Enum;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Metrics;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Application.Services.Metrics;

public class MetaMetricsService : IMetaMetrics
{
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<MetaMetricsService> _logger;
    private readonly IUnitOfWork _uof;

    public MetaMetricsService(ICurrentUser currentUser, ILogger<MetaMetricsService> logger, IUnitOfWork uof)
    {
        _currentUser = currentUser;
        _logger = logger;
        _uof = uof;
    }

    public async Task<ApiResponse<MetasKPIsNoMesDTO>> GetKPIsMetasNoMes(int month)
    {
        var query = _uof.MetaRepository
                    .Query()
                    .AsNoTracking();

        var metas = await query.ToListAsync();

        if (metas is null)
            return ApiResponse<MetasKPIsNoMesDTO>.Fail($"{ResultMessages.EmptyMetaCollection}");

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


        var metasKpisDTO = new MetasKPIsNoMesDTO
        {
            MetasIniciadasNoMes = metasIniciadasNoMes,
            TotalConcluidasNoMes = metasConcluidasNoMes,
            TotalPendentesNoMes = metasPendentesNoMes,
            TotalCanceladasNoMes = metasCanceladasNoMes,
            ValorTotalRestanteParaCompletar = valorRestanteParaCompletarTodas,
            MediaDiasParaCompletar = mediaDias
        };


        return ApiResponse<MetasKPIsNoMesDTO>.Ok( metasKpisDTO, "KPIs");
    }

    public async Task<ApiResponse<MetasKPIsGeralDTO>> GetKPIsMetas()
    {
        var query = _uof.MetaRepository
                    .Query()
                    .AsNoTracking();

        var metas = await query.ToListAsync();

        if (metas is null)
            return ApiResponse<MetasKPIsGeralDTO>.Fail($"{ResultMessages.EmptyMetaCollection}");

        int metasIniciadasNoMes = metas
                                 .Count();


        int metasConcluidasNoMes = metas
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


        var metasKpisDTO = new MetasKPIsGeralDTO
        {
            MetasIniciadas = metasIniciadasNoMes,
            TotalConcluidas = metasConcluidasNoMes,
            TotalPendentes = metasPendentesNoMes,
            TotalCanceladas = metasCanceladasNoMes,
            ValorTotalRestanteParaCompletarTodas = valorRestanteParaCompletarTodas,
        };


        return ApiResponse<MetasKPIsGeralDTO>.Ok(metasKpisDTO, "KPIs");
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


    public async Task<ApiResponse<MetasResumoPeriodoDTO>> GetMetricsMetasNoPeriodo(DateTime start, DateTime end)
    {
        var query = _uof.MetaRepository
                      .Query()
                      .Where(m => m.DataInicio >= start)
                      .Where(m => m.DataAlvo <= end)
                      .AsNoTracking();

        var metas = await query.ToListAsync();

        if (metas is null)
            return ApiResponse<MetasResumoPeriodoDTO>.Fail($"{ResultMessages.EmptyMetaCollection}");

        decimal totalValorAlvoNoPeriodo = metas
                                      .Sum(m => m.ValorAlvo);

        decimal totalAportadoNoPeriodo = metas
                                     .Sum(m => m.ValorAtual);


        var metasResumoPeriodoDTO = new MetasResumoPeriodoDTO
        {
            TotalValorAlvoNoPeriodo = totalValorAlvoNoPeriodo,
            TotalAportadoNoPeriodo = totalAportadoNoPeriodo,
        };

        return ApiResponse<MetasResumoPeriodoDTO>.Ok(metasResumoPeriodoDTO, $"Metas mensais");
    }

    public async Task<ApiResponse<MetasKPIsPeriodoDTO>> GetKPIsMetasNoPeriodo(DateTime start, DateTime end)
    {
        var query = _uof.MetaRepository
              .Query()
              .Where(m => m.DataInicio >= start)
              .Where(m => m.DataAlvo <= end)
              .AsNoTracking();

        var metas = await query.ToListAsync();

        if (metas is null)
            return ApiResponse<MetasKPIsPeriodoDTO>.Fail($"{ResultMessages.EmptyMetaCollection}");

        int metasIniciadasNoPeriodo = metas
                                 .Count();


        int metasConcluidasNoPeriodo = metas
                                  .Where(m => m.Status == Status.Concluido)
                                  .Where(m => m.DataConclusao != null)
                                  .Count();


        int metasPendentesNoPeriodo = metas
                                 .Where(m => m.Status == Status.EmAberto)
                                 .Count();

        int metasCanceladasNoPeriodo = metas
                                  .Where(m => m.Status == Status.Cancelada)
                                  .Count();

        decimal valorRestanteParaCompletarTodas = metas
                                                 .Where(m => m.Status == Status.EmAberto)
                                                 .Sum(m => m.ValorRestanteParaCompletar());

        double mediaDias = metas
                           .Where(m => m.Status == Status.Concluido)
                           .Where(m => m.DataConclusao != null)
                           .Average(m => (m.DataConclusao!.Value - m.DataInicio).TotalDays);


        var metasKpisPeriodoDTO = new MetasKPIsPeriodoDTO
        {
            MetasIniciadasNoPeriodo = metasIniciadasNoPeriodo,
            TotalConcluidasNoPeriodo = metasConcluidasNoPeriodo,
            TotalPendentesNoPeriodo = metasPendentesNoPeriodo,
            TotalCanceladasNoPeriodo = metasCanceladasNoPeriodo,
            ValorTotalRestanteParaCompletar = valorRestanteParaCompletarTodas,
        };


        return ApiResponse<MetasKPIsPeriodoDTO>.Ok(metasKpisPeriodoDTO, "KPIs");
    }
}
