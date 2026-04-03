using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Consolidado;
using FinanceEdgeTrack.Domain.Interfaces.Metrics;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using FinanceEdgeTrack.Domain.Interfaces.Services.Cache;
using FinanceEdgeTrack.Domain.Interfaces.Services.Dashboard;
using FinanceEdgeTrack.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using System.Diagnostics.Metrics;

namespace FinanceEdgeTrack.Application.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly ICarteiraMetrics _carteiraMetrics;
    private readonly IDespesaMetrics _despesaMetrics;
    private readonly IMetaMetrics _metaMetrics;
    private readonly IReceitaMetrics _receitaMetrics;
    private readonly ICurrentUser _currentUser;
    private readonly ICacheService _cacheService;

    public DashboardService(ICarteiraMetrics carteiraMetrics, IDespesaMetrics despesaMetrics, IMetaMetrics metaMetrics,
                            IReceitaMetrics receitaMetrics, ICurrentUser currentUser, ICacheService cacheService)
    {
        _carteiraMetrics = carteiraMetrics;
        _despesaMetrics = despesaMetrics;
        _metaMetrics = metaMetrics;
        _receitaMetrics = receitaMetrics;
        _currentUser = currentUser;
        _cacheService = cacheService;
    }

    private string CacheKey()
    {
        return _cacheService.SetCacheKey(_currentUser.UserId);
    }

    // TODO - Implementação service do dashboard
    // Utilizar Cache para recuperar informações do dashboard in memory caso necessário.

    public async Task<ApiResponse<DashboardConsolidadoDTO>> GetDashboardGeral()
    {
        var saldoTask = _carteiraMetrics.GetSaldoAtualUser();
        var despesaMetricsTask = _despesaMetrics.GetDespesaMetricsTotal();
        var receitaMetricsTask = _receitaMetrics.GetReceitaMetrics();
        var metaMetricsTask = _metaMetrics.GetMetricsMetas();
        var metaKpisTask = _metaMetrics.GetKPIsMetas();

        await Task.WhenAll(saldoTask, despesaMetricsTask, receitaMetricsTask, metaMetricsTask, metaKpisTask);

        var saldoAtual = saldoTask.Result;
        var despesaMetrics = despesaMetricsTask.Result;
        var receitaMetrics = receitaMetricsTask.Result;
        var metaMetrics = metaMetricsTask.Result;
        var metaKpis = metaKpisTask.Result;

        if (saldoAtual?.Data is null ||
        despesaMetrics?.Data is null ||
        receitaMetrics?.Data is null ||
        metaMetrics?.Data is null ||
        metaKpis?.Data is null)
        {
            return ApiResponse<DashboardConsolidadoDTO>.Fail(ResultMessages.ErrorToCreateDashboard);
        }

        var dashboardDTO = new DashboardConsolidadoDTO
        {
            Saldo = saldoAtual.Data.SaldoAtual,
            ValorTotalGasto = despesaMetrics.Data.ValorTotalDespesas,
            ValorTotalGastosFixos = despesaMetrics.Data.ValorTotalDespesasFixas,
            ValorTotalRecebido = receitaMetrics.Data.ValorTotalReceitas,
            ValorAlvoEmMetas = metaMetrics.Data.TotalValorAlvo,
            ValorAportadoEmMetas = metaMetrics.Data.TotalAportado,
            PorcentagemConcluida = metaMetrics.Data.PercentualConclusao,
            ValorRestanteParaCompletarTodas = metaKpis.Data.ValorTotalRestanteParaCompletarTodas,
            TotalConcluidas = metaKpis.Data.TotalConcluidas,
            TotalPendentes = metaKpis.Data.TotalPendentes,
            TotalCanceladas = metaKpis.Data.TotalCanceladas
        };

        await _cacheService.SetAsync(CacheKey(), dashboardDTO, TimeSpan.FromMinutes(10));

        return ApiResponse<DashboardConsolidadoDTO>.Ok(dashboardDTO);
    }

    public async Task<ApiResponse<DashboardConsolidadoNoMesDTO>> GetDashboardMensal(int year, int month)
    {
        var date = DateTime.DaysInMonth(year, month);

        var saldoTask = _carteiraMetrics.GetSaldoAtualUser();
        var despesaMetricsTask = _despesaMetrics.GetDespesaMetricsNoMes(date);
        var receitaMetricsTask = _receitaMetrics.GetReceitaMetricsNoMes(date);
        var metaMetricsTask = _metaMetrics.GetMetricsMetasNoMes(date);
        var metaKpisTask = _metaMetrics.GetKPIsMetasNoMes(date);

        await Task.WhenAll(saldoTask, despesaMetricsTask, receitaMetricsTask, metaMetricsTask, metaKpisTask);

        var saldoAtual = saldoTask.Result;
        var despesaMetrics = despesaMetricsTask.Result;
        var receitaMetrics = receitaMetricsTask.Result;
        var metaMetrics = metaMetricsTask.Result;
        var metaKpis = metaKpisTask.Result;

        if (saldoAtual?.Data is null ||
        despesaMetrics?.Data is null ||
        receitaMetrics?.Data is null ||
        metaMetrics?.Data is null ||
        metaKpis?.Data is null)
        {
            return ApiResponse<DashboardConsolidadoNoMesDTO>.Fail(ResultMessages.ErrorToCreateDashboard);
        }

        var dashboardMensalDTO = new DashboardConsolidadoNoMesDTO
        {
            Saldo = saldoAtual.Data.SaldoAtual,
            MediaGastosDiarioNoMes = (despesaMetrics.Data.ValorTotalDespesasNoMes / date),
            ValorTotalGastoNoMes = despesaMetrics.Data.ValorTotalDespesasNoMes,
            ValorTotalRecebidoNoMes = receitaMetrics.Data.ValorTotalReceitasNoMes,
            ValorAlvoEmMetasNoMes = metaMetrics.Data.TotalValorAlvoNoMes,
            ValorAportadoEmMetasNoMes = metaMetrics.Data.TotalAportadoNoMes,
            MediaDiasParaConclusao = (int)metaKpis.Data.MediaDiasParaCompletar,
            TotalConcluidasNoMes = metaKpis.Data.TotalConcluidasNoMes,
            TotalPendentesNoMes = metaKpis.Data.TotalPendentesNoMes,
            TotalCanceladasNoMes = metaKpis.Data.TotalCanceladasNoMes
        };

        await _cacheService.SetAsync(CacheKey(), dashboardMensalDTO, TimeSpan.FromMinutes(10));

        return ApiResponse<DashboardConsolidadoNoMesDTO>.Ok(dashboardMensalDTO);
    }

    public Task<ApiResponse<DashboardConsolidadoPeriodoDTO>> GetDashboardPeriodo(DateTime dataInicio, DateTime dataFim)
    {
        throw new NotImplementedException();
    }

}
