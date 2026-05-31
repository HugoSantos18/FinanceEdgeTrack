using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Dashboard.Metas;
using FinanceEdgeTrack.Domain.Enums;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Application.Interfaces.Services.Metrics;
using FinanceEdgeTrack.Application.Interfaces.Services.Auth;
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

    public async Task<ApiResponse<MetasKPIsNoMesDTO>> GetKPIsMetasNoMes(int year, int month)
    {
        var startDate = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
        var endDate = DateTime.SpecifyKind(startDate.AddMonths(1).AddTicks(-1), DateTimeKind.Utc);

        var metas = await _uof.MetaRepository
            .Query()
            .AsNoTracking()
            .Where(m => m.DataInicio >= startDate && m.DataInicio <= endDate)
            .ToListAsync();

        if (metas.Count == 0)
        {
            _logger.LogInformation("Nenhuma meta encontrada para {Month}/{Year}", month, year);
            
            var metasKpisEmptyDTO = new MetasKPIsNoMesDTO
            {
                MetasIniciadasNoMes = 0,
                TotalConcluidasNoMes = 0,
                TotalPendentesNoMes = 0,
                TotalCanceladasNoMes = 0,
                ValorTotalRestanteParaCompletar = 0,
                MediaDiasParaCompletar = 0
            };
            
            return ApiResponse<MetasKPIsNoMesDTO>.Ok(metasKpisEmptyDTO, $"Nenhuma KPI encontrada em {month}-{year}");
        }

        int metasIniciadasNoMes = metas.Count;

        int metasConcluidasNoMes = metas
            .Count(m => m.Status == Status.Concluido && m.DataConclusao != null);

        int metasPendentesNoMes = metas
            .Count(m => m.Status == Status.EmAberto);

        int metasCanceladasNoMes = metas
            .Count(m => m.Status == Status.Cancelada);

        decimal valorRestanteParaCompletarTodas = metas
            .Where(m => m.Status == Status.EmAberto)
            .Sum(m => m.ValorRestanteParaCompletar());

        var metasConcluidas = metas
            .Where(m => m.Status == Status.Concluido && m.DataConclusao != null)
            .ToList();

        double mediaDias = metasConcluidas.Count > 0
            ? metasConcluidas.Average(m => (m.DataConclusao!.Value - m.DataInicio).TotalDays)
            : 0;


        var metasKpisDTO = new MetasKPIsNoMesDTO
        {
            MetasIniciadasNoMes = metasIniciadasNoMes,
            TotalConcluidasNoMes = metasConcluidasNoMes,
            TotalPendentesNoMes = metasPendentesNoMes,
            TotalCanceladasNoMes = metasCanceladasNoMes,
            ValorTotalRestanteParaCompletar = valorRestanteParaCompletarTodas,
            MediaDiasParaCompletar = mediaDias
        };


        return ApiResponse<MetasKPIsNoMesDTO>.Ok(metasKpisDTO, "KPIs");
    }

    public async Task<ApiResponse<MetasKPIsGeralDTO>> GetKPIsMetas()
    {
        var metas = await _uof.MetaRepository
            .Query()
            .AsNoTracking()
            .ToListAsync();

        if (metas.Count == 0)
        {
            _logger.LogInformation("Nenhuma meta registrada para KPIs gerais");
            
            var metasKpisEmptyDTO = new MetasKPIsGeralDTO
            {
                MetasIniciadas = 0,
                TotalConcluidas = 0,
                TotalPendentes = 0,
                TotalCanceladas = 0,
                ValorTotalRestanteParaCompletarTodas = 0,
            };

            return ApiResponse<MetasKPIsGeralDTO>.Ok(metasKpisEmptyDTO, $"Sem nenhum KPI");
        }

        int totalMetas = metas.Count;

        int metasConcluidasNoMes = metas
            .Count(m => m.Status == Status.Concluido && m.DataConclusao != null);

        int metasPendentesNoMes = metas
            .Count(m => m.Status == Status.EmAberto);

        int metasCanceladasNoMes = metas
            .Count(m => m.Status == Status.Cancelada);

        decimal valorRestanteParaCompletarTodas = metas
            .Where(m => m.Status == Status.EmAberto)
            .Sum(m => m.ValorRestanteParaCompletar());

        var metasConcluidas = metas
            .Where(m => m.Status == Status.Concluido && m.DataConclusao != null)
            .ToList();

        double mediaDias = metasConcluidas.Count > 0
            ? metasConcluidas.Average(m => (m.DataConclusao!.Value - m.DataInicio).TotalDays)
            : 0;


        var metasKpisDTO = new MetasKPIsGeralDTO
        {
            MetasIniciadas = totalMetas,
            TotalConcluidas = metasConcluidasNoMes,
            TotalPendentes = metasPendentesNoMes,
            TotalCanceladas = metasCanceladasNoMes,
            ValorTotalRestanteParaCompletarTodas = valorRestanteParaCompletarTodas,
        };


        return ApiResponse<MetasKPIsGeralDTO>.Ok(metasKpisDTO, "KPIs");
    }

    public async Task<ApiResponse<MetasResumoGeralDTO>> GetMetricsMetas()
    {
        var metas = await _uof.MetaRepository
            .Query()
            .AsNoTracking()
            .ToListAsync();

        if (metas.Count == 0)
        {
            _logger.LogInformation("Nenhuma meta foi registrada ainda");
            var metasResumoEmptyDTO = new MetasResumoGeralDTO
            {
                TotalValorAlvo = 0,
                TotalAportado = 0,
                PercentualConclusao = 0
            };

            return ApiResponse<MetasResumoGeralDTO>.Ok(metasResumoEmptyDTO, $"Nenhuma meta encontrada");
        }

        decimal totalValorAlvo = metas.Sum(m => m.ValorAlvo);
        decimal totalAportado = metas.Sum(m => m.ValorAtual);
        int totalConcluidas = metas.Count(m => m.Status == Status.Concluido);

        decimal percentualConcluido = ((decimal)totalConcluidas / metas.Count) * 100;

        var metasResumoDTO = new MetasResumoGeralDTO
        {
            TotalValorAlvo = totalValorAlvo,
            TotalAportado = totalAportado,
            PercentualConclusao = percentualConcluido
        };

        return ApiResponse<MetasResumoGeralDTO>.Ok(metasResumoDTO, "Todas as suas metas");
    }

    public async Task<ApiResponse<MetasResumoMensalDTO>> GetMetricsMetasNoMes(int year, int month)
    {
        var startDate = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
        var endDate = DateTime.SpecifyKind(startDate.AddMonths(1).AddTicks(-1), DateTimeKind.Utc);

        var metas = await _uof.MetaRepository
            .Query()
            .AsNoTracking()
            .Where(m => m.DataInicio >= startDate && m.DataInicio <= endDate)
            .ToListAsync();

        if (metas.Count == 0)
        {
            _logger.LogInformation("Nenhuma meta encontrada para {Month}/{Year}", month, year);
            var metasResumoMensalEmptyDTO = new MetasResumoMensalDTO
            {
                TotalValorAlvoNoMes = 0,
                TotalAportadoNoMes = 0,
            };

            return ApiResponse<MetasResumoMensalDTO>.Ok(metasResumoMensalEmptyDTO, $"Sem metas no mês");
        }
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
        var startUtc = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(end, DateTimeKind.Utc);

        var metas = await _uof.MetaRepository
            .Query()
            .AsNoTracking()
            .Where(m => m.DataInicio >= startUtc && m.DataAlvo <= endUtc)
            .ToListAsync();

        if (metas.Count == 0)
        {
            _logger.LogInformation("Nenhuma meta encontrada no período: {Start} - {End}", start, end);

            var metasResumoPeriodoEmptyDTO = new MetasResumoPeriodoDTO
            {
                TotalValorAlvoNoPeriodo = 0,
                TotalAportadoNoPeriodo = 0,
            };

            return ApiResponse<MetasResumoPeriodoDTO>.Ok(metasResumoPeriodoEmptyDTO, $"Sem nenhuma meta nesse período");
        }

        decimal totalValorAlvoNoPeriodo = metas
                                      .Sum(m => m.ValorAlvo);

        decimal totalAportadoNoPeriodo = metas
                                     .Sum(m => m.ValorAtual);


        var metasResumoPeriodoDTO = new MetasResumoPeriodoDTO
        {
            TotalValorAlvoNoPeriodo = totalValorAlvoNoPeriodo,
            TotalAportadoNoPeriodo = totalAportadoNoPeriodo,
        };

        return ApiResponse<MetasResumoPeriodoDTO>.Ok(metasResumoPeriodoDTO, $"Metas encontradas no período: {start} - {end}");
    }

    public async Task<ApiResponse<MetasKPIsPeriodoDTO>> GetKPIsMetasNoPeriodo(DateTime start, DateTime end)
    {
        var startUtc = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(end, DateTimeKind.Utc);

        var metas = await _uof.MetaRepository
            .Query()
            .AsNoTracking()
            .Where(m => m.DataInicio >= startUtc && m.DataAlvo <= endUtc)
            .ToListAsync();

        if (metas.Count == 0)
        {
            _logger.LogInformation("Nenhuma meta encontrada no período: {Start} - {End}", start, end);

            var metasKpisPeriodoEmptyDTO = new MetasKPIsPeriodoDTO
            {
                MetasIniciadasNoPeriodo = 0,
                TotalConcluidasNoPeriodo = 0,
                TotalPendentesNoPeriodo = 0,
                TotalCanceladasNoPeriodo = 0,
                ValorTotalRestanteParaCompletar = 0,
            };
            return ApiResponse<MetasKPIsPeriodoDTO>.Ok(metasKpisPeriodoEmptyDTO, $"Sem KPIs encontrados nesse período");
        }

        int metasIniciadasNoPeriodo = metas.Count;

        int metasConcluidasNoPeriodo = metas
            .Count(m => m.Status == Status.Concluido && m.DataConclusao != null);

        int metasPendentesNoPeriodo = metas
            .Count(m => m.Status == Status.EmAberto);

        int metasCanceladasNoPeriodo = metas
            .Count(m => m.Status == Status.Cancelada);

        decimal valorRestanteParaCompletarTodas = metas
            .Where(m => m.Status == Status.EmAberto)
            .Sum(m => m.ValorRestanteParaCompletar());

        var metasConcluidas = metas
            .Where(m => m.Status == Status.Concluido && m.DataConclusao != null)
            .ToList();

        double mediaDias = metasConcluidas.Count > 0
            ? metasConcluidas.Average(m => (m.DataConclusao!.Value - m.DataInicio).TotalDays)
            : 0;


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
